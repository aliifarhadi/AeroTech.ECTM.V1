using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance
{
    public interface IGetEnginePeriodPerformanceService
    {
        Task<BackofficePeriodPerformanceDto> ExecuteAsync(long engineId, DateOnly? from, DateOnly? to, SnapshotKind kind, string? selector, CancellationToken cancellationToken = default);
    }
}
