using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight
{
    public interface IDryRunPhasesOnFlightService
    {
        Task<BackofficePhaseDryRunDto> ExecuteAsync(string kind, long documentId, long flightId, CancellationToken cancellationToken = default);
    }
}
