using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record UncorroboratedClimbInDescentUnknown(double? MaxSec, PhasePredicate? CorroboratedBy, PhaseUnknownReason? UnknownReason) : PhasePostProcessor
    {
        public const string Operator = "UNCORROBORATED_CLIMB_IN_DESCENT_UNKNOWN";

        public override string Name => Operator;

        public override void Apply(PhaseFrame frame, PhaseLabels labels)
        {
            var runs = labels.Runs(frame);

            for (var position = 1; position < runs.Count - 1; position++)
            {
                var run = runs[position];

                if (!IsShortClimbBetweenDescents(frame, runs, position) || IsCorroborated(frame, run))
                    continue;

                for (var index = run.Start; index <= run.End; index++)
                    labels.MarkUnknown(index, UnknownReason!.Value, Operator);
            }
        }

        public override void Check(RuleDocumentCheck check, string path)
        {
            check.Positive(MaxSec, $"{path}.maxSec");
            check.Required(UnknownReason, $"{path}.unknownReason");
            check.ProducesLabel(FlightPhaseCode.UNKNOWN);

            if (CorroboratedBy is not null)
                CorroboratedBy.Check(check, $"{path}.corroboratedBy");
        }

        private bool IsShortClimbBetweenDescents(PhaseFrame frame, IReadOnlyList<LabelRun> runs, int position)
        {
            var run = runs[position];

            if (run.Label != FlightPhaseCode.CLIMB)
                return false;

            if (runs[position - 1].Label != FlightPhaseCode.DESCENT || runs[position + 1].Label != FlightPhaseCode.DESCENT)
                return false;

            return frame.RunSeconds(run.Start, run.End) <= MaxSec;
        }

        private bool IsCorroborated(PhaseFrame frame, LabelRun run)
        {
            if (CorroboratedBy is null)
                return false;

            for (var index = run.Start; index <= run.End; index++)
            {
                if (CorroboratedBy.Holds(frame, index))
                    return true;
            }

            return false;
        }
    }
}
