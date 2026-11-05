using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics.Backoffice
{
    public sealed class BackofficeGetFlightPhaseMetricsQueryHandler : IRequestHandler<BackofficeGetFlightPhaseMetricsQuery, BackofficeFlightPhaseMetricsDto>
    {
        private readonly IGetFlightPhaseMetricsService _service;

        public BackofficeGetFlightPhaseMetricsQueryHandler(IGetFlightPhaseMetricsService service) => _service = service;

        public Task<BackofficeFlightPhaseMetricsDto> Handle(BackofficeGetFlightPhaseMetricsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, query.Group, cancellationToken);
    }
}
