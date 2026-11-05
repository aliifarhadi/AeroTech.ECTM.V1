using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AbsComparePredicate(string? Channel, CompareOperator? Op, double? Value, bool? NullAllowed) : PhasePredicate
    {
        public const string Operator = "ABS_COMPARE";

        public override bool Holds(PhaseFrame frame, int index)
        {
            if (frame.Number(Channel!, index) is not { } observed)
                return NullAllowed == true;

            return Comparison.Holds(Math.Abs(observed), Op!.Value, Value!.Value);
        }

        public override PredicateTrace Trace(PhaseFrame frame, int index)
            => new(Operator, Holds(frame, index), Channel, Op, Value, frame.Number(Channel!, index), NullAllowed);

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.NumericChannel(Channel, $"{path}.channel");
            check.Required(Op, $"{path}.op");
            check.NotNegative(Value, $"{path}.value");
        }
    }
}
