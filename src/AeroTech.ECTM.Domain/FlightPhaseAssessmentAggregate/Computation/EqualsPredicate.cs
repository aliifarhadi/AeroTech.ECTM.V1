using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record EqualsPredicate(string? Channel, string? Value) : PhasePredicate
    {
        public const string Operator = "EQUALS";

        public override bool Holds(PhaseFrame frame, int index)
            => string.Equals(frame.Text(Channel!, index), Value, StringComparison.Ordinal);

        public override PredicateTrace Trace(PhaseFrame frame, int index)
            => new(Operator, Holds(frame, index), Channel, Expected: Value, ObservedText: frame.Text(Channel!, index));

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.TextChannel(Channel, $"{path}.channel");
            check.Literal(Channel, Value, $"{path}.value");
        }
    }
}
