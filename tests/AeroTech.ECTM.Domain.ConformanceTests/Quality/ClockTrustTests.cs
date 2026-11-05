using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public sealed class ClockTrustTests
{
    [Fact]
    public void Clock_01_two_clocks_that_disagree_make_time_untrusted_with_an_offset_histogram()
    {
        var clock = ClockOf([0, 1, 2], ["10:00:05", "10:00:06", "10:00:07"], ["10:00:00", "10:00:01", "10:00:02"]);

        Assert.Equal(3, clock.DisagreeRows);
        Assert.Equal(new Dictionary<string, int> { ["5"] = 3 }, clock.OffsetHistogram);
        Assert.Equal(TimeTrust.UNTRUSTED, clock.TimeTrust);
    }

    [Theory]
    [InlineData("24:00:00")]
    [InlineData("10:60:00")]
    [InlineData("10:00:5")]
    [InlineData("10:00")]
    [InlineData(null)]
    public void Clock_02_an_invalid_or_missing_clock_value_makes_time_untrusted(string? invalid)
    {
        var clock = ClockOf([0, 1, 2], ["10:00:00", invalid, "10:00:02"], ["10:00:00", "10:00:01", "10:00:02"]);

        Assert.Equal(1, clock.InvalidOrMissingRows);
        Assert.Equal(TimeTrust.UNTRUSTED, clock.TimeTrust);
    }

    [Fact]
    public void Clock_02_a_one_digit_hour_is_valid_by_owner_ruling_and_invalid_in_the_oracle_format()
    {
        string?[] oneDigitHour = ["3:16:00", "3:16:01", "3:16:02"];

        var accepted = ClockOf([0, 1, 2], oneDigitHour, oneDigitHour);
        var oracle = ClockOf([0, 1, 2], oneDigitHour, oneDigitHour, acceptOneDigitHour: false);

        Assert.Equal((0, TimeTrust.TRUSTED), (accepted.InvalidOrMissingRows, accepted.TimeTrust));
        Assert.Equal((3, TimeTrust.UNTRUSTED), (oracle.InvalidOrMissingRows, oracle.TimeTrust));
    }

    [Fact]
    public void Clock_03_clocks_that_agree_and_step_with_elapsed_are_trusted_across_midnight_and_gaps()
    {
        var clock = ClockOf([0, 1, 5, 6], ["23:59:58", "23:59:59", "00:00:03", "00:00:04"], ["23:59:58", "23:59:59", "00:00:03", "00:00:04"]);

        Assert.Equal((0, 0, 0, 0), (clock.DisagreeRows, clock.InvalidOrMissingRows, clock.PrimaryNotInStepRows, clock.SecondaryNotInStepRows));
        Assert.Equal(TimeTrust.TRUSTED, clock.TimeTrust);
    }

    [Fact]
    public void Clock_03_a_clock_that_does_not_step_with_elapsed_is_counted_within_the_tolerance()
    {
        string?[] stuck = ["10:00:00", "10:00:00", "10:00:02"];
        string?[] stepping = ["10:00:00", "10:00:01", "10:00:02"];

        var strict = ClockOf([0, 1, 2], stuck, stepping);
        var tolerant = ClockOf([0, 1, 2], stuck, stepping, stepToleranceSec: 1);

        Assert.Equal(2, strict.PrimaryNotInStepRows);
        Assert.Equal(TimeTrust.UNTRUSTED, strict.TimeTrust);
        Assert.Equal(0, tolerant.PrimaryNotInStepRows);
    }

    private static ClockEvidence ClockOf(double?[] elapsed, string?[] primary, string?[] secondary, double stepToleranceSec = 0, bool acceptOneDigitHour = true)
    {
        var rules = QualityParity.SeedRules with { Clock = new(stepToleranceSec, acceptOneDigitHour) };
        var input = new QualityInput(
            elapsed.Length,
            elapsed,
            new Dictionary<string, IReadOnlyList<double?>>(),
            new Dictionary<string, IReadOnlyList<string?>> { ["UTC_PRIMARY"] = primary, ["UTC_SECONDARY"] = secondary },
            [],
            HasBlockingValidation: false);

        return QualityEvaluator.Evaluate(rules, input).Clock!;
    }
}
