using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetFlightPhases
{
    public interface IGetFlightPhasesService
    {
        Task<BackofficeFlightPhasesDto> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
