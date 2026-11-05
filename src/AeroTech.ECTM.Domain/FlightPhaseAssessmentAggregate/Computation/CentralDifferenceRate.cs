using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record CentralDifferenceRate(
        string? Code,
        string? Parameter,
        double? HalfWindowSec,
        double? NearestToleranceSec,
        double? PerSeconds) : DerivedChannel(Code)
    {
        public const string Operator = "CENTRAL_DIFFERENCE_RATE";
        public const string NoSampleNearWindowEdge = "NO_SAMPLE_NEAR_WINDOW_EDGE";

        public override IReadOnlyList<double?> Compute(PhaseFrame frame)
        {
            var values = new double?[frame.Count];

            for (var index = 0; index < frame.Count; index++)
                values[index] = RateAt(frame, index).Rate;

            return values;
        }

        public override DerivedValueTrace Trace(PhaseFrame frame, int index)
        {
            var (rate, before, after) = RateAt(frame, index);

            if (rate is null)
                return new DerivedValueTrace(Code!, Operator, null, [], NoSampleNearWindowEdge);

            return new DerivedValueTrace(Code!, Operator, rate, [before!.Value, after!.Value], null);
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.NumericChannel(Parameter, $"{path}.parameter");
            check.Positive(HalfWindowSec, $"{path}.halfWindowSec");
            check.NotNegative(NearestToleranceSec, $"{path}.nearestToleranceSec");
            check.Positive(PerSeconds, $"{path}.perSeconds");
        }

        private (double? Rate, int? Before, int? After) RateAt(PhaseFrame frame, int index)
        {
            var source = frame.NumbersOf(Parameter!);
            var before = Nearest(frame, source, index, frame.Elapsed[index] - HalfWindowSec!.Value);
            var after = Nearest(frame, source, index, frame.Elapsed[index] + HalfWindowSec.Value);

            if (before is not { } first || after is not { } last || first >= last)
                return (null, before, after);

            var rate = (source[last]!.Value - source[first]!.Value) * PerSeconds!.Value / (frame.Elapsed[last] - frame.Elapsed[first]);

            return (rate, first, last);
        }

        private int? Nearest(PhaseFrame frame, IReadOnlyList<double?> source, int index, double target)
        {
            var next = frame.FirstAtOrAfter(target);
            int? nearest = null;

            foreach (var candidate in new[] { next - 1, next })
            {
                if (!IsUsable(frame, source, index, candidate, target))
                    continue;

                if (nearest is null || Distance(frame, candidate, target) < Distance(frame, nearest.Value, target))
                    nearest = candidate;
            }

            return nearest;
        }

        private bool IsUsable(PhaseFrame frame, IReadOnlyList<double?> source, int index, int candidate, double target)
        {
            if (candidate < 0 || candidate >= frame.Count)
                return false;

            if (!frame.SameBlock(candidate, index) || source[candidate] is null)
                return false;

            return Distance(frame, candidate, target) <= NearestToleranceSec;
        }

        private static double Distance(PhaseFrame frame, int candidate, double target) => Math.Abs(frame.Elapsed[candidate] - target);
    }
}
