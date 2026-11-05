using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance
{
    public interface IGetModelPeriodPerformanceService
    {
        Task<BackofficeModelPeriodPerformanceDto> ExecuteAsync(long engineModelId, DateOnly? from, DateOnly? to, SnapshotKind kind, string? selector, CancellationToken cancellationToken = default);
    }
}
