using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record PhaseRuleSetHeader(
        string Code,
        string Title,
        string Description,
        CalibrationStatus CalibrationStatus,
        EngineeringSignOff EngineeringSignOff)
    {
        public static PhaseRuleSetHeader Of(PhaseRuleSetContent rules)
            => new(rules.RuleSetCode!, rules.Title!, rules.Description!, rules.CalibrationStatus!.Value, rules.EngineeringSignOff!.Value);
    }
}
