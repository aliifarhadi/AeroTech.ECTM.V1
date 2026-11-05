using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record InMaskPredicate(string? Mask) : PhasePredicate
    {
        public const string Operator = "IN_MASK";

        public override bool Holds(PhaseFrame frame, int index) => frame.InMask(Mask!, index);

        public override PredicateTrace Trace(PhaseFrame frame, int index)
            => new(Operator, Holds(frame, index), Reference: Mask);

        public override void Check(RuleDocumentCheck check, string path) => check.Mask(Mask, $"{path}.mask");
    }
}
