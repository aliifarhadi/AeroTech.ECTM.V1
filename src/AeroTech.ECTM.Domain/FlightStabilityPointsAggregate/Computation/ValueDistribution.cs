namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ValueDistribution(string Parameter, int N, double? Mean, double? Median, double? P10, double? P90, double? Min, double? Max)
    {
        public static ValueDistribution Of(string parameter, IEnumerable<double> values)
        {
            var sorted = values.Order().ToList();

            return new ValueDistribution(
                parameter,
                sorted.Count,
                SampleStatistics.Mean(sorted),
                SampleStatistics.Median(sorted),
                SampleStatistics.NearestRankPercentile(sorted, 10),
                SampleStatistics.NearestRankPercentile(sorted, 90),
                sorted.Count == 0 ? null : sorted[0],
                sorted.Count == 0 ? null : sorted[^1]);
        }
    }
}
