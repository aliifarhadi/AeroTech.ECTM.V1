using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase
{
    public interface IExplainFlightPhaseService
    {
        Task<BackofficePhaseExplanationDto> ExecuteAsync(long flightId, int sampleIndex, CancellationToken cancellationToken = default);
    }
}
