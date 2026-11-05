using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record NotPredicate(PhasePredicate? Predicate) : PhasePredicate
    {
        public const string Operator = "NOT";

        public override bool Holds(PhaseFrame frame, int index) => !Predicate!.Holds(frame, index);

        public override PredicateTrace Trace(PhaseFrame frame, int index)
        {
            var inner = Predicate!.Trace(frame, index);

            return new PredicateTrace(Operator, !inner.Holds, Of: [inner]);
        }

        public override void Check(PhaseRuleSetCheck check, string path) => CheckPresent(Predicate, check, $"{path}.predicate");
    }
}
