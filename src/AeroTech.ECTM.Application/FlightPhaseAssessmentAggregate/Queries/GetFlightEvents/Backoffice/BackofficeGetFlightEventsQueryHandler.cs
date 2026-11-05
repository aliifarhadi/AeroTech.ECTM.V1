using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents.Backoffice
{
    public sealed class BackofficeGetFlightEventsQueryHandler : IRequestHandler<BackofficeGetFlightEventsQuery, BackofficeFlightEventsDto>
    {
        private readonly IGetFlightEventsService _service;

        public BackofficeGetFlightEventsQueryHandler(IGetFlightEventsService service) => _service = service;

        public Task<BackofficeFlightEventsDto> Handle(BackofficeGetFlightEventsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
