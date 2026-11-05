using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeFlightEventsDto(
        long FlightId,
        long AnalysisRevisionId,
        long PhaseRuleSetDocumentId,
        int PhaseRuleSetVersion,
        IReadOnlyList<BackofficeEventSpanDto> Spans,
        IReadOnlyList<BackofficeEventCoverageDto> Coverage)
    {
        public static BackofficeFlightEventsDto From(FlightPhaseAssessment assessment)
        {
            var result = assessment.ReadResult();

            return new BackofficeFlightEventsDto(
                assessment.FlightId,
                assessment.AnalysisRevisionId,
                assessment.PhaseRuleSetDocumentId,
                assessment.PhaseRuleSetVersion,
                result.Events.Select(BackofficeEventSpanDto.From).ToList(),
                result.EventCoverage.Select(BackofficeEventCoverageDto.From).ToList());
        }
    }
}
