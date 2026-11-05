using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record RuleDocumentHeader(
        string Code,
        string Title,
        string Description,
        CalibrationStatus CalibrationStatus,
        EngineeringSignOff EngineeringSignOff)
    {
        public static RuleDocumentHeader Of(PhaseRuleSetContent rules)
            => new(rules.RuleSetCode!, rules.Title!, rules.Description!, rules.CalibrationStatus!.Value, rules.EngineeringSignOff!.Value);

        public static RuleDocumentHeader Of(SnapshotRulesContent rules)
            => new(rules.RulesCode!, rules.Title!, rules.Description!, rules.CalibrationStatus!.Value, rules.EngineeringSignOff!.Value);
    }
}
