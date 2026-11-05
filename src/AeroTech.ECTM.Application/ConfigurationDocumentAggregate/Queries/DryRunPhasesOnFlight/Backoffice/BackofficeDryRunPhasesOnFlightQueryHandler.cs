using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight.Backoffice
{
    public sealed class BackofficeDryRunPhasesOnFlightQueryHandler : IRequestHandler<BackofficeDryRunPhasesOnFlightQuery, BackofficePhaseDryRunDto>
    {
        private readonly IDryRunPhasesOnFlightService _service;

        public BackofficeDryRunPhasesOnFlightQueryHandler(IDryRunPhasesOnFlightService service) => _service = service;

        public Task<BackofficePhaseDryRunDto> Handle(BackofficeDryRunPhasesOnFlightQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.DocumentId, query.FlightId, cancellationToken);
    }
}
