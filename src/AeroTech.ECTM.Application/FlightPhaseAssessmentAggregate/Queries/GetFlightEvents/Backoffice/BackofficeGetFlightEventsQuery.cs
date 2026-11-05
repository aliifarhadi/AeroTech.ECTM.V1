using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents.Backoffice
{
    public sealed record BackofficeGetFlightEventsQuery(long FlightId) : IRequest<BackofficeFlightEventsDto>;
}
