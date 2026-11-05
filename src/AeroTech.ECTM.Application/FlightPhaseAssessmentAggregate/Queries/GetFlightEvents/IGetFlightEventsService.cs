using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightEvents
{
    public interface IGetFlightEventsService
    {
        Task<BackofficeFlightEventsDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
