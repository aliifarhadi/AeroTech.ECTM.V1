using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase.Backoffice
{
    public sealed record BackofficeExplainFlightPhaseQuery(long FlightId, int SampleIndex) : IRequest<BackofficePhaseExplanationDto>;
}
