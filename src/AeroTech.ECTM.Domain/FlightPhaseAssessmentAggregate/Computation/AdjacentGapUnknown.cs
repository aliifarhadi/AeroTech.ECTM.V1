using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record AdjacentGapUnknown(PhaseUnknownReason? UnknownReason) : PhasePostProcessor
    {
        public const string Operator = "ADJACENT_GAP_UNKNOWN";

        public override string Name => Operator;

        public override void Apply(PhaseFrame frame, PhaseLabels labels)
        {
            foreach (var gapStart in frame.GapStarts)
            {
                labels.MarkUnknown(gapStart - 1, UnknownReason!.Value, Operator);
                labels.MarkUnknown(gapStart, UnknownReason.Value, Operator);
            }
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.Required(UnknownReason, $"{path}.unknownReason");
            check.ProducesLabel(FlightPhaseCode.UNKNOWN);
        }
    }
}
