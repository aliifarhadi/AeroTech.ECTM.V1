using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseExplanationDto(
        long FlightId,
        long AnalysisRevisionId,
        long PhaseRuleSetDocumentId,
        int PhaseRuleSetVersion,
        int SampleIndex,
        double Elapsed,
        EnumValueDto Phase,
        EnumValueDto? UnknownReason,
        string? FiredRuleId,
        EnumValueDto RuleLabel,
        string? ChangedBy,
        BackofficePhaseSegmentDto Segment,
        IReadOnlyList<BackofficeLabelRuleTraceDto> Rules,
        IReadOnlyList<ChannelValue> Inputs,
        IReadOnlyList<DerivedValueTrace> Derived,
        IReadOnlyList<MaskTrace> Masks,
        IReadOnlyList<BackofficeAnchorDto> Anchors)
    {
        public static BackofficePhaseExplanationDto From(long flightId, long analysisRevisionId, long phaseRuleSetDocumentId, int phaseRuleSetVersion, PhaseExplanation explanation)
            => new(
                flightId,
                analysisRevisionId,
                phaseRuleSetDocumentId,
                phaseRuleSetVersion,
                explanation.SampleIndex,
                explanation.Elapsed,
                EnumValueDto.Of(explanation.Label),
                EnumValueDto.OfNullable(explanation.UnknownReason),
                explanation.FiredRuleId,
                EnumValueDto.Of(explanation.RuleLabel),
                explanation.ChangedBy,
                BackofficePhaseSegmentDto.From(explanation.Segment),
                explanation.Rules.Select(BackofficeLabelRuleTraceDto.From).ToList(),
                explanation.Inputs,
                explanation.Derived,
                explanation.Masks,
                explanation.Anchors.Select(BackofficeAnchorDto.From).ToList());
    }
}
