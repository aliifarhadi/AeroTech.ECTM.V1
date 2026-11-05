using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases.Backoffice
{
    public sealed record BackofficeGetFlightPhasesQuery(long FlightId) : IRequest<BackofficeFlightPhasesDto>;
}
