using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.SourceFileAggregate;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightDetailDto(
        long Id,
        BackofficeFlightIdentityDto Identity,
        bool DistinctFlightConfirmed,
        string? IdentityNote,
        EnumValueDto State,
        EnumValueDto? StateBeforeHold,
        string? HoldNote,
        DateTimeOffset? HeldUtc,
        BackofficeSourceFileDto SourceFile,
        BackofficeConfigurationVersionsDto Versions,
        int SampleCount,
        double? FirstElapsed,
        double? LastElapsed,
        int BlockingCount,
        int WarningCount,
        int InfoCount,
        IReadOnlyList<BackofficeValidationFindingDto> Issues,
        IReadOnlyList<BackofficeFlightChannelDto> Channels,
        long ImportBatchId,
        long ImportItemId,
        DateTimeOffset ImportedUtc)
    {
        public static BackofficeFlightDetailDto From(Flight flight, SourceFile sourceFile, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                flight.Id,
                BackofficeFlightIdentityDto.From(flight, serialByEngine),
                flight.DistinctFlightConfirmed,
                flight.IdentityNote,
                EnumValueDto.Of(flight.State),
                EnumValueDto.OfNullable(flight.StateBeforeHold),
                flight.HoldNote,
                flight.HeldAt,
                BackofficeSourceFileDto.From(sourceFile),
                BackofficeConfigurationVersionsDto.From(flight.Versions),
                flight.SampleCount,
                flight.FirstElapsed,
                flight.LastElapsed,
                flight.BlockingCount,
                flight.WarningCount,
                flight.InfoCount,
                flight.Issues
                    .OrderBy(issue => issue.Severity)
                    .ThenBy(issue => issue.Code, StringComparer.Ordinal)
                    .ThenBy(issue => issue.ParameterKey, StringComparer.Ordinal)
                    .Select(BackofficeValidationFindingDto.From)
                    .ToList(),
                flight.Channels
                    .OrderBy(channel => channel.ProfileOrdinal)
                    .Select(BackofficeFlightChannelDto.From)
                    .ToList(),
                flight.ImportBatchId,
                flight.ImportItemId,
                flight.ImportedAt);
    }
}
