namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class SampleStatistics
    {
        public static double? Mean(IReadOnlyList<double> values)
        {
            if (values.Count == 0)
                return null;

            return CompensatedSum.Of(values) / values.Count;
        }

        public static double? Median(IReadOnlyList<double> sorted)
        {
            if (sorted.Count == 0)
                return null;

            var middle = sorted.Count / 2;

            return sorted.Count % 2 == 1 ? sorted[middle] : (sorted[middle - 1] + sorted[middle]) / 2;
        }

        public static double? NearestRankPercentile(IReadOnlyList<double> sorted, double percentile)
        {
            if (sorted.Count == 0)
                return null;

            var rank = (int)Math.Ceiling(percentile / 100 * sorted.Count);

            return sorted[Math.Clamp(rank, 1, sorted.Count) - 1];
        }

        public static double? SampleStandardDeviation(IReadOnlyList<double> values)
        {
            if (values.Count < 2 || Mean(values) is not { } mean)
                return null;

            var squares = CompensatedSum.Of(values.Select(value => (value - mean) * (value - mean)));

            return Math.Sqrt(squares / (values.Count - 1));
        }
    }
}
