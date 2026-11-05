using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed record MedianSmooth(
        string? Code,
        string? Source,
        int? Points,
        int? MinValid,
        IReadOnlyList<string>? NullInsideMasks) : DerivedChannel(Code)
    {
        public const string Operator = "MEDIAN_SMOOTH";
        public const string TooFewValidValues = "TOO_FEW_VALID_VALUES";

        public override IReadOnlyList<double?> Compute(PhaseFrame frame)
        {
            var values = new double?[frame.Count];

            for (var index = 0; index < frame.Count; index++)
                values[index] = Trace(frame, index).Value;

            return values;
        }

        public override DerivedValueTrace Trace(PhaseFrame frame, int index)
        {
            if (MaskAt(frame, index) is { } mask)
                return new DerivedValueTrace(Code!, Operator, null, [], mask);

            var window = WindowOf(frame, index);

            if (window.Count < MinValid)
                return new DerivedValueTrace(Code!, Operator, null, window, TooFewValidValues);

            var source = frame.NumbersOf(Source!);
            var median = Median(window.Select(sample => source[sample]!.Value));

            return new DerivedValueTrace(Code!, Operator, median, window, null);
        }

        public override void Check(PhaseRuleSetCheck check, string path)
        {
            check.NumericChannel(Source, $"{path}.source");
            check.AtLeast(Points, 1, $"{path}.points");
            check.AtLeast(MinValid, 1, $"{path}.minValid");

            if (MinValid > Points)
                check.Issue($"{path}.minValid", PhaseRuleSetValidator.ValueInvalid, "minValid cannot exceed points.");

            check.Entries(NullInsideMasks, $"{path}.nullInsideMasks", allowEmpty: true);

            for (var item = 0; item < (NullInsideMasks?.Count ?? 0); item++)
                check.Mask(NullInsideMasks![item], $"{path}.nullInsideMasks[{item}]");
        }

        private string? MaskAt(PhaseFrame frame, int index)
            => NullInsideMasks!.FirstOrDefault(mask => frame.InMask(mask, index));

        private List<int> WindowOf(PhaseFrame frame, int index)
        {
            var source = frame.NumbersOf(Source!);
            var half = Points!.Value / 2;
            var first = Math.Max(0, index - half);
            var last = Math.Min(frame.Count - 1, index + half);
            var window = new List<int>();

            for (var sample = first; sample <= last; sample++)
            {
                if (frame.SameBlock(sample, index) && source[sample] is not null)
                    window.Add(sample);
            }

            return window;
        }

        private static double Median(IEnumerable<double> values)
        {
            var sorted = values.Order().ToList();
            var middle = sorted.Count / 2;

            if (sorted.Count % 2 == 1)
                return sorted[middle];

            return (sorted[middle - 1] + sorted[middle]) / 2;
        }
    }
}
