using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto
{
    public sealed record BackofficeRecomputeDto(
        long FlightId,
        bool DryRun,
        BackofficePhaseDryRunDto? Preview,
        BackofficeAnalysisRevisionDto? Revision,
        long? ProcessingJobId);
}
