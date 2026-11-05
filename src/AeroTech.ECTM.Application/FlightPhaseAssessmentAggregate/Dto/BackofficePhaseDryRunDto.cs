using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseDryRunDto(
        long FlightId,
        long? CurrentRevisionId,
        int? CurrentRevisionNumber,
        long? CurrentPhaseRuleSetDocumentId,
        int? CurrentPhaseRuleSetVersion,
        EnumValueDto? CurrentGrade,
        long ProposedQualityRulesDocumentId,
        int ProposedQualityRulesVersion,
        long? ProposedPhaseRuleSetDocumentId,
        int? ProposedPhaseRuleSetVersion,
        EnumValueDto? ProposedPhaseRuleSetStatus,
        EnumValueDto ProposedGrade,
        int ChangedSamples,
        double ChangedPercent,
        IReadOnlyList<BackofficePhaseChangeDto> Changes,
        IReadOnlyList<BackofficeAnchorChangeDto> AnchorChanges,
        IReadOnlyList<BackofficePhaseCoverageDto> CurrentCoverage,
        IReadOnlyList<BackofficePhaseCoverageDto> ProposedCoverage,
        IReadOnlyList<BackofficePhaseSegmentDto> ProposedSegments,
        IReadOnlyList<BackofficeAnchorDto> ProposedAnchors,
        BackofficePhaseReferenceDto? ProposedReference);
}
