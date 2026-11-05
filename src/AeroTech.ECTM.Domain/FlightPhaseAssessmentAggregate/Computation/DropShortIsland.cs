using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record DropShortIsland(double? MinSec, IReadOnlyList<FlightPhaseCode>? Exempt, PhaseUnknownReason? UnknownReason) : PhasePostProcessor
    {
        public const string Operator = "DROP_SHORT_ISLAND";

        public override string Name => Operator;

        public override void Apply(PhaseFrame frame, PhaseLabels labels)
        {
            foreach (var run in labels.Runs(frame))
            {
                if (Exempt!.Contains(run.Label) || frame.RunSeconds(run.Start, run.End) >= MinSec)
                    continue;

                for (var index = run.Start; index <= run.End; index++)
                    labels.MarkUnknown(index, UnknownReason!.Value, Operator);
            }
        }

        public override void Check(RuleDocumentCheck check, string path)
        {
            check.Positive(MinSec, $"{path}.minSec");
            check.Entries(Exempt, $"{path}.exempt", allowEmpty: true);
            check.Required(UnknownReason, $"{path}.unknownReason");
            check.ProducesLabel(FlightPhaseCode.UNKNOWN);
        }
    }
}
