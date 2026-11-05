using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public sealed class StuckSensorTests
{
    private const int Samples = 60;

    [Fact]
    public void Egt_that_stays_identical_while_n1_moves_is_stuck()
    {
        var n1 = Enumerable.Range(0, Samples).Select(index => (double?)(60 + (index * 0.2))).ToArray();

        var span = Assert.Single(StuckSpansOf(n1, egt: 650));

        Assert.Equal(("EGT_C[1]", 0, Samples - 1), (span.ParameterKeys.Single(), span.StartIndex, span.EndIndexInclusive));
        Assert.Equal((Samples, 30), (span.Actual, span.Threshold));
        Assert.Equal("The value stayed 650 while N1 moved 11.8 %.", span.Note);
    }

    [Fact]
    public void Egt_that_stays_identical_in_steady_cruise_is_not_stuck()
    {
        var n1 = Enumerable.Range(0, Samples).Select(index => (double?)(index % 2 == 0 ? 85.00 : 85.02)).ToArray();

        Assert.Empty(StuckSpansOf(n1, egt: 650));
    }

    [Fact]
    public void Nothing_is_stuck_while_the_engine_is_below_the_live_n1()
    {
        var n1 = Enumerable.Range(0, Samples).Select(index => (double?)(20 + (index * 0.2))).ToArray();

        Assert.Empty(StuckSpansOf(n1, egt: 400));
    }

    private static IEnumerable<QualitySpan> StuckSpansOf(double?[] n1, double egt)
    {
        var elapsed = Enumerable.Range(0, Samples).Select(index => (double?)index).ToArray();
        var egtValues = Enumerable.Repeat((double?)egt, Samples).ToArray();
        var input = new QualityInput(
            Samples,
            elapsed,
            new Dictionary<string, IReadOnlyList<double?>> { ["N1_PCT[1]"] = n1, ["EGT_C[1]"] = egtValues },
            new Dictionary<string, IReadOnlyList<string?>>(),
            [
                new ChannelStatistics("N1_PCT[1]", "N1_PCT", 11, Samples, Samples, 0),
                new ChannelStatistics("EGT_C[1]", "EGT_C", 15, Samples, Samples, 0)
            ],
            HasBlockingValidation: false);

        return QualityEvaluator.Evaluate(QualityParity.SeedRules, input).Spans.Where(span => span.Code == QualitySpanCodes.SensorStuck);
    }
}
