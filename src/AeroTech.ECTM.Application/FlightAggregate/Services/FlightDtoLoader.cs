using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightAggregate.Services
{
    public sealed class FlightDtoLoader
    {
        private readonly IEngineRepository _engines;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly IFlightQualityRepository _qualities;

        public FlightDtoLoader(IEngineRepository engines, ISourceFileRepository sourceFiles, IFlightQualityRepository qualities)
        {
            _engines = engines;
            _sourceFiles = sourceFiles;
            _qualities = qualities;
        }

        public async Task<IReadOnlyDictionary<long, string>> SerialsOfAsync(IEnumerable<long?> engineIds, CancellationToken cancellationToken = default)
        {
            var ids = engineIds
                .Where(engineId => engineId is not null)
                .Select(engineId => engineId!.Value)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return new Dictionary<long, string>();

            var engines = await _engines.ListByIdsAsync(ids, cancellationToken);

            return engines.ToDictionary(engine => engine.Id, engine => engine.SerialNumber);
        }

        public async Task<BackofficeFlightDetailDto> LoadDetailAsync(Flight flight, CancellationToken cancellationToken = default)
        {
            var sourceFile = await _sourceFiles.GetAsync(flight.SourceFileId, cancellationToken)
                             ?? throw ExceptionFactory.FlightNotFound(flight.Id);
            var serialByEngine = await SerialsOfAsync(flight.Engines.Select(engine => engine.EngineId), cancellationToken);
            var quality = await _qualities.FindCurrentAsync(flight.Id, cancellationToken);

            return BackofficeFlightDetailDto.From(flight, sourceFile, quality?.Grade, serialByEngine);
        }

        public async Task<IReadOnlyList<BackofficeFlightSummaryDto>> LoadSummariesAsync(IReadOnlyCollection<Flight> flights, CancellationToken cancellationToken = default)
        {
            var engineIds = flights.SelectMany(flight => flight.Engines).Select(engine => engine.EngineId);
            var serialByEngine = await SerialsOfAsync(engineIds, cancellationToken);
            var qualities = await _qualities.ListCurrentAsync(flights.Select(flight => flight.Id).ToList(), cancellationToken);
            var gradeByFlight = qualities.ToDictionary(quality => quality.FlightId, quality => (QualityGrade?)quality.Grade);

            return flights
                .Select(flight => BackofficeFlightSummaryDto.From(flight, gradeByFlight.GetValueOrDefault(flight.Id), serialByEngine))
                .ToList();
        }
    }
}
