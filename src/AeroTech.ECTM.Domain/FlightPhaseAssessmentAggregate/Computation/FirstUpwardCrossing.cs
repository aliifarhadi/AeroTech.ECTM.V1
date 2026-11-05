using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record FirstUpwardCrossing(
        string? Code,
        AnchorBasis? Basis,
        IReadOnlyList<AnchorBasis>? FallbackBases,
        string? Channel,
        double? AboveValue,
        PhasePredicate? At,
        double? SustainSec) : PhaseAnchor(Code, Basis, FallbackBases)
    {
        public const string Operator = "FIRST_UPWARD_CROSSING";

        public override AnchorResult Find(PhaseFrame frame)
        {
            var values = frame.NumbersOf(Channel!);

            for (var index = 1; index < frame.Count; index++)
            {
                if (IsCrossing(frame, values, index) && StaysAbove(frame, values, index))
                    return Found(frame, values, index);
            }

            return new AnchorResult(Code!, Operator, null, null, BasisWhenNotFound(frame), Channel!);
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            CheckBases(check, path);
            check.NumericChannel(Channel, $"{path}.channel");
            check.Required(AboveValue, $"{path}.aboveValue");
            check.NotNegative(SustainSec, $"{path}.sustainSec");

            if (At is not null)
                At.Check(check, $"{path}.at");
        }

        private bool IsCrossing(PhaseFrame frame, IReadOnlyList<double?> values, int index)
        {
            if (frame.StartsAfterGap(index))
                return false;

            if (values[index - 1] is not { } before || values[index] is not { } at)
                return false;

            if (before > AboveValue || at <= AboveValue)
                return false;

            return At is null || At.Holds(frame, index);
        }

        private bool StaysAbove(PhaseFrame frame, IReadOnlyList<double?> values, int index)
        {
            for (var sample = index; sample < frame.Count && frame.Elapsed[sample] - frame.Elapsed[index] < SustainSec; sample++)
            {
                if (!frame.SameBlock(sample, index) || values[sample] is not { } value || value <= AboveValue)
                    return false;
            }

            return true;
        }

        private AnchorResult Found(PhaseFrame frame, IReadOnlyList<double?> values, int index)
            => new(Code!, Operator, index, frame.Elapsed[index], Basis!.Value, Channel!,
                NumberBefore: values[index - 1],
                NumberAt: values[index],
                At: At?.Trace(frame, index));
    }
}
