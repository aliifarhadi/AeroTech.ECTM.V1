using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record SustainedTransition(
        string? Code,
        AnchorBasis? Basis,
        IReadOnlyList<AnchorBasis>? FallbackBases,
        string? Channel,
        string? From,
        string? To,
        double? HoldSec,
        TransitionPick? Pick,
        string? AfterAnchor) : PhaseAnchor(Code, Basis, FallbackBases)
    {
        public const string Operator = "SUSTAINED_TRANSITION";

        public override AnchorResult Find(PhaseFrame frame)
        {
            var earliest = AfterAnchor is null ? null : frame.AnchorIndex(AfterAnchor);
            int? chosen = null;

            for (var index = 1; index < frame.Count; index++)
            {
                if (!IsTransition(frame, index) || !IsHeld(frame, index))
                    continue;

                if (earliest is { } anchor && index <= anchor)
                    continue;

                chosen = index;

                if (Pick == TransitionPick.FIRST)
                    break;
            }

            if (chosen is not { } found)
                return new AnchorResult(Code!, Operator, null, null, BasisWhenNotFound(frame), Channel!);

            return new AnchorResult(Code!, Operator, found, frame.Elapsed[found], Basis!.Value, Channel!,
                TextBefore: frame.Text(Channel!, found - 1),
                TextAt: frame.Text(Channel!, found));
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            CheckBases(check, path);
            check.TextChannel(Channel, $"{path}.channel");
            check.Literal(Channel, From, $"{path}.from");
            check.Literal(Channel, To, $"{path}.to");
            check.NotNegative(HoldSec, $"{path}.holdSec");
            check.Required(Pick, $"{path}.pick");

            if (AfterAnchor is not null)
                check.Anchor(AfterAnchor, $"{path}.afterAnchor");
        }

        private bool IsTransition(PhaseFrame frame, int index)
        {
            if (frame.StartsAfterGap(index))
                return false;

            return frame.Text(Channel!, index - 1) == From && frame.Text(Channel!, index) == To;
        }

        private bool IsHeld(PhaseFrame frame, int index)
        {
            for (var sample = index; sample < frame.Count && frame.Elapsed[sample] - frame.Elapsed[index] < HoldSec; sample++)
            {
                if (!frame.SameBlock(sample, index) || frame.Text(Channel!, sample) != To)
                    return false;
            }

            return true;
        }
    }
}
