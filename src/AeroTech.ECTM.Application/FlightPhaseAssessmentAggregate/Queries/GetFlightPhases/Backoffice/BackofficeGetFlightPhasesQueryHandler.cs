using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases.Backoffice
{
    public sealed class BackofficeGetFlightPhasesQueryHandler : IRequestHandler<BackofficeGetFlightPhasesQuery, BackofficeFlightPhasesDto>
    {
        private readonly IGetFlightPhasesService _service;

        public BackofficeGetFlightPhasesQueryHandler(IGetFlightPhasesService service) => _service = service;

        public Task<BackofficeFlightPhasesDto> Handle(BackofficeGetFlightPhasesQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
