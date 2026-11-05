using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AnyPredicate(IReadOnlyList<PhasePredicate>? Of) : PhasePredicate
    {
        public const string Operator = "ANY";

        public override bool Holds(PhaseFrame frame, int index) => Of!.Any(predicate => predicate.Holds(frame, index));

        public override PredicateTrace Trace(PhaseFrame frame, int index)
        {
            var children = Of!.Select(predicate => predicate.Trace(frame, index)).ToList();

            return new PredicateTrace(Operator, children.Any(child => child.Holds), Of: children);
        }

        public override void Check(RuleDocumentCheck check, string path) => CheckEach(Of, check, $"{path}.of");
    }
}
