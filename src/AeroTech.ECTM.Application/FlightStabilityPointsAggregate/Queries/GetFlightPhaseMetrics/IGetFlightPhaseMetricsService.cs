using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics
{
    public interface IGetFlightPhaseMetricsService
    {
        Task<BackofficeFlightPhaseMetricsDto> ExecuteAsync(long flightId, MetricGroupKind? group, CancellationToken cancellationToken = default);
    }
}
