using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record BaselineRulesContent(
        string? RulesCode,
        string? Title,
        string? Description,
        CalibrationStatus? CalibrationStatus,
        EngineeringSignOff? EngineeringSignOff,
        string? Source,
        IReadOnlyList<BaselineConditionRule>? Conditions,
        IReadOnlyList<RuleDecision>? Decisions);
}
