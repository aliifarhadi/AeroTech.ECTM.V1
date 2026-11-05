using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record BackFromAnchorWhile(string? Code, string? Anchor, double? LookbackSec, PhasePredicate? While) : PhaseCondition(Code)
    {
        public const string Operator = "BACK_FROM_ANCHOR_WHILE";

        public override bool[] Members(PhaseFrame frame)
        {
            var members = new bool[frame.Count];

            if (WalkOf(frame) is { } walk)
                PhaseFrame.Mark(members, walk.Start, walk.End);

            return members;
        }

        public override ConditionTrace Trace(PhaseFrame frame, int index)
        {
            var walk = WalkOf(frame);

            return new ConditionTrace(
                Code!,
                Operator,
                frame.InCondition(Code!, index),
                While!.Trace(frame, index),
                RequiredSec: LookbackSec,
                Anchor: Anchor,
                AnchorIndex: frame.AnchorIndex(Anchor!),
                WindowStart: walk?.Start,
                WindowEnd: walk?.End);
        }

        public override void Check(RuleDocumentCheck check, string path)
        {
            check.Anchor(Anchor, $"{path}.anchor");
            check.Positive(LookbackSec, $"{path}.lookbackSec");
            PhasePredicate.CheckPresent(While, check, $"{path}.while");
        }

        private (int Start, int End)? WalkOf(PhaseFrame frame)
        {
            if (frame.AnchorIndex(Anchor!) is not { } anchor)
                return null;

            var start = anchor;

            while (start - 1 >= 0 && IsStillBefore(frame, start - 1, anchor) && While!.Holds(frame, start - 1))
                start--;

            return start < anchor ? (start, anchor - 1) : null;
        }

        private bool IsStillBefore(PhaseFrame frame, int index, int anchor)
            => frame.SameBlock(index, anchor) && frame.Elapsed[anchor] - frame.Elapsed[index] <= LookbackSec;
    }
}
