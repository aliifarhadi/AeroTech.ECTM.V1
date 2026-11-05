using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class PhaseComparison
    {
        public static PhaseDiff Compare(PhaseResult? current, PhaseResult? proposed)
        {
            var before = LabelsOf(current);
            var after = LabelsOf(proposed);
            var count = Math.Max(before.Length, after.Length);
            var changes = new List<PhaseChange>();
            var changedSamples = 0;
            var index = 0;

            while (index < count)
            {
                if (LabelAt(before, index) == LabelAt(after, index))
                {
                    index++;
                    continue;
                }

                var start = index;

                while (index + 1 < count && IsSameChange(before, after, start, index + 1))
                    index++;

                changes.Add(new PhaseChange(start, index, LabelAt(before, start), LabelAt(after, start)));
                changedSamples += index - start + 1;
                index++;
            }

            var percent = count == 0 ? 0 : Math.Round(100.0 * changedSamples / count, 2);

            return new PhaseDiff(count, changedSamples, percent, changes, AnchorChangesOf(current, proposed));
        }

        private static bool IsSameChange(FlightPhaseCode[] before, FlightPhaseCode[] after, int start, int index)
            => LabelAt(before, index) == LabelAt(before, start) && LabelAt(after, index) == LabelAt(after, start);

        private static FlightPhaseCode? LabelAt(FlightPhaseCode[] labels, int index) => index < labels.Length ? labels[index] : null;

        private static FlightPhaseCode[] LabelsOf(PhaseResult? result)
        {
            if (result is null)
                return [];

            var labels = new FlightPhaseCode[result.Segments.Count == 0 ? 0 : result.SampleCount];

            foreach (var segment in result.Segments)
            {
                for (var index = segment.StartIndex; index <= segment.EndIndexInclusive; index++)
                    labels[index] = segment.PhaseCode;
            }

            return labels;
        }

        private static IReadOnlyList<AnchorChange> AnchorChangesOf(PhaseResult? current, PhaseResult? proposed)
        {
            var codes = (current?.Anchors ?? []).Select(anchor => anchor.Code)
                .Union((proposed?.Anchors ?? []).Select(anchor => anchor.Code), StringComparer.Ordinal);
            var changes = new List<AnchorChange>();

            foreach (var code in codes)
            {
                var before = current?.AnchorOf(code);
                var after = proposed?.AnchorOf(code);

                if (before?.Index == after?.Index && before?.Basis == after?.Basis)
                    continue;

                changes.Add(new AnchorChange(code, before?.Index, after?.Index, before?.Basis, after?.Basis));
            }

            return changes;
        }
    }
}
