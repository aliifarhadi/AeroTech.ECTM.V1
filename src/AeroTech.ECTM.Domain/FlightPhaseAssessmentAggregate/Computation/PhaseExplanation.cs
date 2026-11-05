using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseExplanation(
        int SampleIndex,
        double Elapsed,
        FlightPhaseCode Label,
        PhaseUnknownReason? UnknownReason,
        string? FiredRuleId,
        FlightPhaseCode RuleLabel,
        string? ChangedBy,
        PhaseSegment Segment,
        IReadOnlyList<LabelRuleTrace> Rules,
        IReadOnlyList<ChannelValue> Inputs,
        IReadOnlyList<DerivedValueTrace> Derived,
        IReadOnlyList<MaskTrace> Masks,
        IReadOnlyList<AnchorResult> Anchors);
}
