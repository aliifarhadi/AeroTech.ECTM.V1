using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record LabelRuleTrace(string RuleId, FlightPhaseCode Label, bool Matched, bool Fired, PredicateTrace When);
}
