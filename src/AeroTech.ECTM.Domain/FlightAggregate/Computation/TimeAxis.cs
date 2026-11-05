namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record TimeAxis(
        double? NominalStep,
        int MissingCount,
        int DuplicateCount,
        int DecreasingCount,
        IReadOnlyList<TimeGap> Gaps)
    {
        private const int StepDecimals = 6;

        public double MissingSeconds => Gaps.Sum(gap => gap.ElapsedTo - gap.ElapsedFrom - NominalStep!.Value);

        public static TimeAxis Of(IReadOnlyList<double?> elapsed)
        {
            var missingCount = elapsed.Count(value => value is null);
            var steps = StepsOf(elapsed);
            var nominalStep = NominalStepOf(steps);
            var duplicateCount = steps.Count(step => step.Seconds == 0);
            var decreasingCount = steps.Count(step => step.Seconds < 0);
            var gaps = new List<TimeGap>();

            if (nominalStep is { } nominal)
            {
                gaps = steps
                    .Where(step => step.Seconds > nominal)
                    .Select(step => new TimeGap(step.AfterSampleIndex, elapsed[step.AfterSampleIndex]!.Value, elapsed[step.AfterSampleIndex + 1]!.Value))
                    .ToList();
            }

            return new TimeAxis(nominalStep, missingCount, duplicateCount, decreasingCount, gaps);
        }

        private static List<Step> StepsOf(IReadOnlyList<double?> elapsed)
        {
            var steps = new List<Step>();

            for (var index = 0; index + 1 < elapsed.Count; index++)
            {
                if (elapsed[index] is not { } current || elapsed[index + 1] is not { } next)
                    continue;

                var seconds = Math.Round(next - current, StepDecimals);

                steps.Add(new Step(index, seconds));
            }

            return steps;
        }

        private static double? NominalStepOf(IReadOnlyList<Step> steps)
        {
            var positiveSteps = steps.Where(step => step.Seconds > 0).ToList();

            if (positiveSteps.Count == 0)
                return null;

            return positiveSteps
                .GroupBy(step => step.Seconds)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key)
                .First()
                .Key;
        }

        private sealed record Step(int AfterSampleIndex, double Seconds);
    }
}
