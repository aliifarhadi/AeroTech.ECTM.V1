using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase.Backoffice
{
    public sealed class BackofficeExplainFlightPhaseQueryHandler : IRequestHandler<BackofficeExplainFlightPhaseQuery, BackofficePhaseExplanationDto>
    {
        private readonly IExplainFlightPhaseService _service;

        public BackofficeExplainFlightPhaseQueryHandler(IExplainFlightPhaseService service) => _service = service;

        public Task<BackofficePhaseExplanationDto> Handle(BackofficeExplainFlightPhaseQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, query.SampleIndex, cancellationToken);
    }
}
