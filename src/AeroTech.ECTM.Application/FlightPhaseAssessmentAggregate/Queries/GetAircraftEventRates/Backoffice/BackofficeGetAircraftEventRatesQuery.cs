using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates.Backoffice
{
    public sealed record BackofficeGetAircraftEventRatesQuery(long AircraftId, DateOnly? From, DateOnly? To) : IRequest<BackofficeAircraftEventRatesDto>;
}
