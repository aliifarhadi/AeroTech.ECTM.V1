using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics.Backoffice
{
    public sealed record BackofficeGetFlightPhaseMetricsQuery(long FlightId, MetricGroupKind? Group) : IRequest<BackofficeFlightPhaseMetricsDto>;
}
