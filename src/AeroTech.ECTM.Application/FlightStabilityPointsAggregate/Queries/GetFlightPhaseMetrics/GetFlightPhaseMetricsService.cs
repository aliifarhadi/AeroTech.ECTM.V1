using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics
{
    public sealed class GetFlightPhaseMetricsService : IGetFlightPhaseMetricsService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public GetFlightPhaseMetricsService(IFlightRepository flights, IFlightStabilityPointsRepository stabilityPoints)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<BackofficeFlightPhaseMetricsDto> ExecuteAsync(long flightId, MetricGroupKind? group, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var points = await _stabilityPoints.FindCurrentAsync(flight.Id, cancellationToken)
                         ?? throw ExceptionFactory.StabilityPointsNotEvaluated(flight.Id);
            var groups = points.ReadPhaseMetrics().Groups.AsEnumerable();

            if (group is { } kind)
                groups = groups.Where(candidate => candidate.Kind == kind);

            return new BackofficeFlightPhaseMetricsDto(
                flight.Id,
                points.AnalysisRevisionId,
                points.SnapshotRulesDocumentId,
                points.SnapshotRulesVersion,
                EnumValueDto.Of(points.Status),
                groups.Select(BackofficePhaseMetricGroupDto.From).ToList());
        }
    }
}
