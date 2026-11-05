using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed class PhaseLabels
    {
        public PhaseLabels(int count)
        {
            Labels = new FlightPhaseCode[count];
            UnknownReasons = new PhaseUnknownReason?[count];
            FiredRules = new string?[count];
            RuleLabels = new FlightPhaseCode[count];
            ChangedBy = new string?[count];
        }

        public FlightPhaseCode[] Labels { get; }

        public PhaseUnknownReason?[] UnknownReasons { get; }

        public string?[] FiredRules { get; }

        public FlightPhaseCode[] RuleLabels { get; }

        public string?[] ChangedBy { get; }

        public void Assign(int index, FlightPhaseCode label, PhaseUnknownReason? unknownReason, string? ruleId)
        {
            Labels[index] = label;
            RuleLabels[index] = label;
            UnknownReasons[index] = unknownReason;
            FiredRules[index] = ruleId;
        }

        public void MarkUnknown(int index, PhaseUnknownReason reason, string postProcessor)
        {
            Labels[index] = FlightPhaseCode.UNKNOWN;
            UnknownReasons[index] = reason;
            ChangedBy[index] = postProcessor;
        }

        public IReadOnlyList<LabelRun> Runs(PhaseFrame frame)
        {
            var runs = new List<LabelRun>();
            var start = 0;

            for (var index = 1; index <= Labels.Length; index++)
            {
                var endsRun = index == Labels.Length || Labels[index] != Labels[start] || frame.StartsAfterGap(index);

                if (!endsRun)
                    continue;

                runs.Add(new LabelRun(Labels[start], start, index - 1));
                start = index;
            }

            return runs;
        }
    }
}
