using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeFlightPhasesDto(
        long Id,
        long FlightId,
        long AnalysisRevisionId,
        int RevisionNumber,
        long PhaseRuleSetDocumentId,
        int PhaseRuleSetVersion,
        BackofficePhaseRuleSetHeaderDto RuleSet,
        BackofficePhaseReferenceDto Reference,
        EnumValueDto Status,
        string? NotEvaluableReason,
        int SampleCount,
        IReadOnlyList<BackofficeAnchorDto> Anchors,
        IReadOnlyList<BackofficePhaseCoverageDto> Coverage,
        IReadOnlyList<BackofficePhaseSegmentDto> Segments,
        IReadOnlyList<BackofficePhaseTransitionDto> Transitions,
        DateTimeOffset EvaluatedUtc)
    {
        public static BackofficeFlightPhasesDto From(FlightPhaseAssessment assessment, AnalysisRevision revision)
        {
            var result = assessment.ReadResult();

            return new BackofficeFlightPhasesDto(
                assessment.Id,
                assessment.FlightId,
                assessment.AnalysisRevisionId,
                revision.Number,
                assessment.PhaseRuleSetDocumentId,
                assessment.PhaseRuleSetVersion,
                BackofficePhaseRuleSetHeaderDto.From(result.RuleSet),
                BackofficePhaseReferenceDto.From(result.Reference),
                EnumValueDto.Of(assessment.Status),
                result.NotEvaluableReason,
                result.SampleCount,
                result.Anchors.Select(BackofficeAnchorDto.From).ToList(),
                result.Coverage.Select(BackofficePhaseCoverageDto.From).ToList(),
                result.Segments.Select(BackofficePhaseSegmentDto.From).ToList(),
                result.Transitions.Select(BackofficePhaseTransitionDto.From).ToList(),
                assessment.EvaluatedAt);
        }
    }
}
