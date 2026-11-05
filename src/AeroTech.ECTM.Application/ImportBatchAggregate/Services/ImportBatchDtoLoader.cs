using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.ImportBatchAggregate;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class ImportBatchDtoLoader
    {
        private readonly IFlightRepository _flights;
        private readonly FlightDtoLoader _flightDtos;
        private readonly IFlightQualityRepository _qualities;

        public ImportBatchDtoLoader(IFlightRepository flights, FlightDtoLoader flightDtos, IFlightQualityRepository qualities)
        {
            _flights = flights;
            _flightDtos = flightDtos;
            _qualities = qualities;
        }

        public async Task<BackofficeImportBatchDto> LoadAsync(
            ImportBatch batch,
            IReadOnlyDictionary<long, ShownRows> rowsByItem,
            CancellationToken cancellationToken = default)
        {
            var items = batch.Items.OrderBy(item => item.Ordinal).ToList();
            var previewByItem = items.ToDictionary(item => item.Id, item => item.ReadPreview());

            var flightIds = items
                .Select(item => item.FlightId ?? item.ExistingFlightId)
                .Where(flightId => flightId is not null)
                .Select(flightId => flightId!.Value)
                .Distinct()
                .ToList();
            var flights = await _flights.ListByIdsAsync(flightIds, cancellationToken);
            var flightById = flights.ToDictionary(flight => flight.Id);
            var qualities = await _qualities.ListCurrentAsync(flightIds, cancellationToken);
            var gradeByFlight = qualities.ToDictionary(quality => quality.FlightId, quality => (QualityGrade?)quality.Grade);

            var previewEngineIds = previewByItem.Values
                .Where(preview => preview.Identity is not null)
                .SelectMany(preview => preview.Identity!.Engines)
                .Select(engine => engine.EngineId);
            var flightEngineIds = flights.SelectMany(flight => flight.Engines).Select(engine => engine.EngineId);
            var serialByEngine = await _flightDtos.SerialsOfAsync(previewEngineIds.Concat(flightEngineIds), cancellationToken);

            var itemDtos = items
                .Select(item => ItemDtoOf(item, previewByItem[item.Id], FlightOf(item, flightById), gradeByFlight, rowsByItem.GetValueOrDefault(item.Id), serialByEngine))
                .ToList();
            var tallies = items
                .GroupBy(item => new { item.Outcome, item.State })
                .Select(group => new ImportItemTally(batch.Id, group.Key.Outcome, group.Key.State, group.Count()))
                .ToList();

            return new BackofficeImportBatchDto(
                batch.Id,
                EnumValueDto.Of(batch.State),
                BackofficeConfigurationVersionsDto.From(batch.Versions),
                batch.DefaultRegistration,
                batch.DefaultYear,
                batch.JobId,
                batch.CreatedAt,
                batch.CommittedAt,
                BackofficeImportCountsDto.From(tallies),
                itemDtos);
        }

        private static Flight? FlightOf(ImportItem item, IReadOnlyDictionary<long, Flight> flightById)
        {
            var flightId = item.FlightId ?? item.ExistingFlightId;

            if (flightId is not { } id)
                return null;

            return flightById.GetValueOrDefault(id);
        }

        private static BackofficeImportItemDto ItemDtoOf(
            ImportItem item,
            ImportItemPreview preview,
            Flight? flight,
            IReadOnlyDictionary<long, QualityGrade?> gradeByFlight,
            ShownRows? rows,
            IReadOnlyDictionary<long, string> serialByEngine)
        {
            BackofficeFlightIdentityDto? identity = null;

            if (preview.Identity is { } previewIdentity)
                identity = BackofficeFlightIdentityDto.From(previewIdentity, serialByEngine);

            if (flight is not null)
                identity = BackofficeFlightIdentityDto.From(flight, serialByEngine);

            var findings = preview.Findings
                .OrderBy(finding => finding.Severity)
                .ThenBy(finding => finding.Code, StringComparer.Ordinal)
                .ThenBy(finding => finding.ParameterKey, StringComparer.Ordinal)
                .Select(BackofficeValidationFindingDto.From)
                .ToList();

            return new BackofficeImportItemDto(
                item.Id,
                item.Ordinal,
                item.FileName,
                item.ContainerName,
                item.Sha256,
                item.SizeBytes,
                EnumValueDto.Of(item.Outcome),
                item.RejectionCode,
                preview.RejectionMessage,
                item.ExistingFlightId,
                EnumValueDto.Of(item.State),
                item.ErrorCode,
                item.FlightId,
                EnumValueDto.OfNullable(flight?.State),
                EnumValueDto.OfNullable(flight is null ? null : gradeByFlight.GetValueOrDefault(flight.Id)),
                identity,
                preview.SampleCount,
                preview.FirstElapsed,
                preview.LastElapsed,
                CountOf(preview, ValidationSeverity.BLOCKING),
                CountOf(preview, ValidationSeverity.WARNING),
                CountOf(preview, ValidationSeverity.INFO),
                findings,
                preview.HeaderMismatches,
                rows?.First ?? [],
                rows?.Last ?? []);
        }

        private static int CountOf(ImportItemPreview preview, ValidationSeverity severity)
            => preview.Findings.Count(finding => finding.Severity == severity);
    }
}
