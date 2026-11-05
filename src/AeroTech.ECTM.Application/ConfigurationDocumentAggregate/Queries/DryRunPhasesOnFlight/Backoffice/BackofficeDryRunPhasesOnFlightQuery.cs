using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight.Backoffice
{
    public sealed record BackofficeDryRunPhasesOnFlightQuery(string Kind, long DocumentId, long FlightId) : IRequest<BackofficePhaseDryRunDto>;
}
