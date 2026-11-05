using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record InConditionPredicate(string? Condition) : PhasePredicate
    {
        public const string Operator = "IN_CONDITION";

        public override bool Holds(PhaseFrame frame, int index) => frame.InCondition(Condition!, index);

        public override PredicateTrace Trace(PhaseFrame frame, int index)
            => new(Operator, Holds(frame, index), Reference: Condition, Condition: frame.TraceCondition(Condition!, index));

        public override void Check(RuleDocumentCheck check, string path) => check.Condition(Condition, $"{path}.condition");
    }
}
