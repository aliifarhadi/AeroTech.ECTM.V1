using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AllPredicate(IReadOnlyList<PhasePredicate>? Of) : PhasePredicate
    {
        public const string Operator = "ALL";

        public override bool Holds(PhaseFrame frame, int index) => Of!.All(predicate => predicate.Holds(frame, index));

        public override PredicateTrace Trace(PhaseFrame frame, int index)
        {
            var children = Of!.Select(predicate => predicate.Trace(frame, index)).ToList();

            return new PredicateTrace(Operator, children.All(child => child.Holds), Of: children);
        }

        public override void Check(RuleDocumentCheck check, string path) => CheckEach(Of, check, $"{path}.of");
    }
}
