using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightSummaryDto(
        long Id,
        DateOnly? FlightDate,
        long? AircraftId,
        string? Registration,
        string? FlightNumber,
        IReadOnlyList<BackofficeEngineAssignmentDto> Engines,
        EnumValueDto State,
        EnumValueDto IdentityStatus,
        int SampleCount,
        int BlockingCount,
        int WarningCount,
        int InfoCount,
        long ImportBatchId,
        DateTimeOffset ImportedUtc)
    {
        public static BackofficeFlightSummaryDto From(Flight flight, IReadOnlyDictionary<long, string> serialByEngine)
            => new(
                flight.Id,
                flight.FlightDate,
                flight.AircraftId,
                flight.Registration,
                flight.FlightNumber,
                flight.Engines
                    .OrderBy(engine => engine.Position)
                    .Select(engine => BackofficeEngineAssignmentDto.From(engine, serialByEngine))
                    .ToList(),
                EnumValueDto.Of(flight.State),
                EnumValueDto.Of(flight.IdentityStatus),
                flight.SampleCount,
                flight.BlockingCount,
                flight.WarningCount,
                flight.InfoCount,
                flight.ImportBatchId,
                flight.ImportedAt);
    }
}
