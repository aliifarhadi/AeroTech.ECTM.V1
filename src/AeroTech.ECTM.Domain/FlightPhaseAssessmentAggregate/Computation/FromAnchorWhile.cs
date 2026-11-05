using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record FromAnchorWhile(string? Code, string? Anchor, PhasePredicate? StartWhen, PhasePredicate? While) : PhaseCondition(Code)
    {
        public const string Operator = "FROM_ANCHOR_WHILE";

        public override bool[] Members(PhaseFrame frame)
        {
            var members = new bool[frame.Count];

            if (WalkOf(frame) is { } walk)
                PhaseFrame.Mark(members, walk.Start, walk.End);

            return members;
        }

        public override ConditionTrace Trace(PhaseFrame frame, int index)
        {
            var anchor = frame.AnchorIndex(Anchor!);
            var walk = WalkOf(frame);
            var gate = anchor is { } anchorIndex ? StartWhen?.Trace(frame, anchorIndex) : null;

            return new ConditionTrace(
                Code!,
                Operator,
                frame.InCondition(Code!, index),
                While!.Trace(frame, index),
                Anchor: Anchor,
                AnchorIndex: anchor,
                WindowStart: walk?.Start,
                WindowEnd: walk?.End,
                Gate: gate);
        }

        public override void Check(RuleDocumentCheck check, string path)
        {
            check.Anchor(Anchor, $"{path}.anchor");

            if (StartWhen is not null)
                StartWhen.Check(check, $"{path}.startWhen");

            PhasePredicate.CheckPresent(While, check, $"{path}.while");
        }

        private (int Start, int End)? WalkOf(PhaseFrame frame)
        {
            if (frame.AnchorIndex(Anchor!) is not { } anchor)
                return null;

            if (StartWhen is not null && !StartWhen.Holds(frame, anchor))
                return null;

            var end = anchor - 1;

            while (end + 1 < frame.Count && frame.SameBlock(end + 1, anchor) && While!.Holds(frame, end + 1))
                end++;

            return end >= anchor ? (anchor, end) : null;
        }
    }
}
