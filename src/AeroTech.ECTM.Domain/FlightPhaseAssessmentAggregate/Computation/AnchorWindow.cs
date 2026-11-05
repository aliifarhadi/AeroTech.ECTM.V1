using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AnchorWindow(string? Code, string? Anchor, double? AfterSec, PhasePredicate? Where) : PhaseCondition(Code)
    {
        public const string Operator = "ANCHOR_WINDOW";

        public override bool[] Members(PhaseFrame frame)
        {
            var members = new bool[frame.Count];

            if (WindowOf(frame) is not { } window)
                return members;

            for (var index = window.Start; index <= window.End; index++)
                members[index] = Where is null || Where.Holds(frame, index);

            return members;
        }

        public override ConditionTrace Trace(PhaseFrame frame, int index)
        {
            var window = WindowOf(frame);

            return new ConditionTrace(
                Code!,
                Operator,
                frame.InCondition(Code!, index),
                Where?.Trace(frame, index),
                Anchor: Anchor,
                AnchorIndex: frame.AnchorIndex(Anchor!),
                WindowStart: window?.Start,
                WindowEnd: window?.End);
        }

        public override void Check(RuleDocumentCheck check, string path)
        {
            check.Anchor(Anchor, $"{path}.anchor");
            check.NotNegative(AfterSec, $"{path}.afterSec");

            if (Where is not null)
                Where.Check(check, $"{path}.where");
        }

        private (int Start, int End)? WindowOf(PhaseFrame frame)
        {
            if (frame.AnchorIndex(Anchor!) is not { } anchor)
                return null;

            var end = anchor;

            while (end + 1 < frame.Count && frame.SameBlock(end + 1, anchor) && frame.Elapsed[end + 1] <= frame.Elapsed[anchor] + AfterSec)
                end++;

            return (anchor, end);
        }
    }
}
