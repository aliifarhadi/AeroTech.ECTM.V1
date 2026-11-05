using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class SeriesSamplerTests
{
    private static readonly double?[] Elapsed = [0, 1, 2, 3, 7, 8, 9];
    private static readonly double?[] Values = [1, 2, null, 4, 5, 6, 7];

    [Fact]
    public void Raw_returns_every_sample_with_a_null_point_at_a_missing_value_and_at_a_gap()
    {
        var series = SeriesSampler.Sample(Elapsed, Values, 0, 6, SeriesSampler.Raw, 2_000);

        SeriesPoint[] expected =
        [
            new(0, 0, 1),
            new(1, 1, 2),
            new(2, 2, null),
            new(3, 3, 4),
            new(null, 4, null),
            new(4, 7, 5),
            new(5, 8, 6),
            new(6, 9, 7)
        ];

        Assert.Equal(expected, series.Points);
        Assert.Equal(new TimeGap(3, 3, 7), Assert.Single(series.Gaps));
        Assert.Equal(7, series.SampleCountInRange);
    }

    [Fact]
    public void M4_stays_within_budget_and_keeps_the_extremes()
    {
        var count = 10_000;
        var elapsed = Enumerable.Range(0, count).Select(index => (double?)index).ToArray();
        var values = Enumerable.Range(0, count).Select(index => (double?)Math.Sin(index / 300.0) * 100).ToArray();
        var budget = 400;

        var series = SeriesSampler.Sample(elapsed, values, 0, count - 1, SeriesSampler.M4, budget);

        Assert.True(series.Points.Count <= budget);
        Assert.Equal(0, series.Points[0].SampleIndex);
        Assert.Equal(count - 1, series.Points[^1].SampleIndex);
        Assert.Equal(values.Max(), series.Points.Max(point => point.Value));
        Assert.Equal(values.Min(), series.Points.Min(point => point.Value));
    }

    [Fact]
    public void M4_never_bridges_a_gap_or_a_missing_value()
    {
        var count = 6_000;
        var elapsed = Enumerable.Range(0, count).Select(index => (double?)(index < 3_000 ? index : index + 10)).ToArray();
        var values = Enumerable.Range(0, count).Select(index => index is >= 1_000 and < 1_050 ? null : (double?)index).ToArray();

        var series = SeriesSampler.Sample(elapsed, values, 0, count - 1, SeriesSampler.M4, 200);
        var points = series.Points;

        for (var index = 0; index + 1 < points.Count; index++)
        {
            var left = points[index];
            var right = points[index + 1];

            if (left.Value is null || right.Value is null)
                continue;

            var crossesMissing = left.SampleIndex < 1_000 && right.SampleIndex >= 1_000;
            var crossesGap = left.SampleIndex < 3_000 && right.SampleIndex >= 3_000;

            Assert.False(crossesMissing || crossesGap, $"points {left} and {right} are joined across a break");
        }
    }

    [Fact]
    public void The_nominal_step_is_the_most_common_step_and_gaps_are_larger_steps()
    {
        var axis = TimeAxis.Of([10, 11, 12, 12, 11, 15, 16]);

        Assert.Equal(1, axis.NominalStep);
        Assert.Equal(1, axis.DuplicateCount);
        Assert.Equal(1, axis.DecreasingCount);
        Assert.Equal(new TimeGap(4, 11, 15), Assert.Single(axis.Gaps));
        Assert.Equal(3, axis.MissingSeconds);
    }
}
