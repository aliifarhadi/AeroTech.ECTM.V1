using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record SamplesWhere(string? Code, PhasePredicate? Predicate) : PhaseCondition(Code)
    {
        public const string Operator = "SAMPLES_WHERE";

        public override bool[] Members(PhaseFrame frame) => frame.SamplesWhere(index => Predicate!.Holds(frame, index));

        public override ConditionTrace Trace(PhaseFrame frame, int index)
            => new(Code!, Operator, frame.InCondition(Code!, index), Predicate!.Trace(frame, index));

        public override void Check(RuleDocumentCheck check, string path)
            => PhasePredicate.CheckPresent(Predicate, check, $"{path}.predicate");
    }
}
