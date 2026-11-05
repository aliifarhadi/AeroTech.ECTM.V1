using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record PhaseRuleSetContent(
        string? RuleSetCode,
        string? Title,
        string? Description,
        CalibrationStatus? CalibrationStatus,
        EngineeringSignOff? EngineeringSignOff,
        string? Source,
        IReadOnlyList<DerivedChannel>? DerivedChannels,
        IReadOnlyList<PhaseAnchor>? Anchors,
        IReadOnlyList<PhaseCondition>? Conditions,
        IReadOnlyList<LabelRule>? LabelRules,
        FallbackLabel? Fallback,
        IReadOnlyList<PhasePostProcessor>? PostProcessing,
        SegmentRules? Segments,
        IReadOnlyList<EventDefinition>? Events,
        IReadOnlyList<RuleDecision>? Decisions);
}
