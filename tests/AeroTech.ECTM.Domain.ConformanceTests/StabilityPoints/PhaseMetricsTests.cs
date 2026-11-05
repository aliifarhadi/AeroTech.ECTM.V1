using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.StabilityPoints;

public sealed class PhaseMetricsTests
{
    [Fact]
    public void The_statistics_follow_their_hand_calculated_definitions()
    {
        IReadOnlyList<double> values = [7, 1, 3, 9, 5, 2, 8, 4, 6, 10];
        var sorted = values.Order().ToList();

        Assert.Equal(5.5, SampleStatistics.Mean(values));
        Assert.Equal(5.5, SampleStatistics.Median(sorted));
        Assert.Equal(10, SampleStatistics.NearestRankPercentile(sorted, 95));
        Assert.Equal(9, SampleStatistics.NearestRankPercentile(sorted, 90));
        Assert.Equal(Math.Sqrt(82.5 / 9), SampleStatistics.SampleStandardDeviation(values)!.Value, 12);
        Assert.Null(SampleStatistics.SampleStandardDeviation([4]));
        Assert.Null(SampleStatistics.Mean([]));
    }

    [Fact]
    public void Compensated_summation_keeps_the_digits_plain_summation_loses()
    {
        double[] values = [1e16, 1, -1e16, 1];

        Assert.Equal(2, CompensatedSum.Of(values));
    }

    [Fact]
    public void Flight_24_has_whole_flight_phase_segment_and_steady_cruise_groups()
    {
        var evaluation = SnapshotParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));
        var groups = evaluation.Metrics.Groups;
        var whole = groups.Single(group => group.Kind == MetricGroupKind.WHOLE_FLIGHT);
        var egt = whole.Parameters.Single(parameter => parameter.ParameterKey == "EGT_C[1]");
        var segments = groups.Where(group => group.Kind == MetricGroupKind.SEGMENT).ToList();

        Assert.Equal(3987, egt.Samples);
        Assert.Equal(egt.Samples, egt.Valued + egt.Missing);
        Assert.True(egt.Min <= egt.Median && egt.Median <= egt.Percentile && egt.Percentile <= egt.Max);
        Assert.Equal(3987, segments.Sum(group => group.EndIndexInclusive - group.StartIndex + 1));
        Assert.Contains(groups, group => group.Kind == MetricGroupKind.PHASE && group.Phase == FlightPhaseCode.CRUISE);
        Assert.Contains(groups, group => group.Kind == MetricGroupKind.STEADY_CRUISE && group.Selector == "OPERATOR_300S" && group.Position == 2);
        Assert.True(whole.Parameters.Single(parameter => parameter.ParameterKey == "PRESSURE_ALT_FT").Masked > 0);
    }
}
