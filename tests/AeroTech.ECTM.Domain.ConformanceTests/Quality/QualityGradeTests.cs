using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public sealed class QualityGradeTests
{
    private static readonly ChannelCoverage Full = new("N1_PCT[1]", true, 100, 100, 100);

    [Fact]
    public void A_clean_recording_with_only_the_allowed_lag_masks_is_grade_A()
        => AssertGrade(QualityGrade.A, [], spans: [Span(QualitySpanCodes.AirGroundLagsRadioHeight)]);

    [Fact]
    public void The_recorder_altitude_jump_confirmed_by_both_channels_does_not_block_grade_A()
    {
        AssertGrade(QualityGrade.A, [], spans: [Span(QualitySpanCodes.AltitudeRateUntrusted) with { Cause = QualitySpanCodes.DiscontinuityConfirmed }]);
        AssertGrade(QualityGrade.B, ["MASK:ALTITUDE_RATE_UNTRUSTED"], spans: [Span(QualitySpanCodes.AltitudeRateUntrusted) with { Cause = QualitySpanCodes.DiscontinuitySingleChannel }]);
    }

    [Fact]
    public void Any_other_mask_a_gap_or_coverage_below_99_percent_is_grade_B()
    {
        AssertGrade(QualityGrade.B, ["MASK:ALTITUDE_RATE_UNTRUSTED"], spans: [Span(QualitySpanCodes.AltitudeRateUntrusted)]);
        AssertGrade(QualityGrade.B, ["ELAPSED_GAP"], gaps: [3943]);
        AssertGrade(QualityGrade.B, ["COVERAGE_BELOW_GRADE_A:N1_PCT[1]"], coverage: Full with { Percent = 98 });
    }

    [Fact]
    public void Coverage_below_95_percent_or_a_partial_start_and_end_is_grade_C()
    {
        AssertGrade(QualityGrade.C, ["COVERAGE_BELOW_WARNING:N1_PCT[1]"], coverage: Full with { Percent = 90 });
        AssertGrade(QualityGrade.C, ["PARTIAL_START_AND_END"], partialStart: true, partialEnd: true);
    }

    [Fact]
    public void Blocking_validation_or_coverage_below_50_percent_is_not_usable()
    {
        AssertGrade(QualityGrade.NOT_USABLE, ["BLOCKING_VALIDATION"], blocking: true);
        AssertGrade(QualityGrade.NOT_USABLE, ["COVERAGE_BELOW_BLOCKING:N1_PCT[1]"], coverage: Full with { Percent = 40 });
    }

    [Fact]
    public void Flight_24_starts_airborne_and_ends_in_the_roll_so_it_is_grade_C()
    {
        var result = QualityParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));

        Assert.Equal(QualityGrade.C, result.Grade);
        Assert.Equal(["PARTIAL_START_AND_END"], result.GradeReasons);
        Assert.All(result.Coverage, channel => Assert.Equal(100, channel.Percent));
    }

    [Fact]
    public void A_reordered_time_axis_gives_a_visible_reason_and_no_numbers()
    {
        var input = QualityInput.From(Recordings.ParseReal(Recordings.Flight24)) with { Elapsed = [0, 2, 1] };

        var result = QualityEvaluator.Evaluate(QualityParity.SeedRules, input);

        Assert.Equal(QualityGrade.NOT_USABLE, result.Grade);
        Assert.Equal([QualityEvaluator.TimeAxisNotIncreasing], result.GradeReasons);
        Assert.Null(result.Clock);
        Assert.Null(result.PartialStart);
        Assert.Null(result.AirGroundLagSec);
    }

    [Fact]
    public void A_missing_channel_is_named_and_never_read_as_zero()
    {
        var recording = Recordings.ParseReal(Recordings.Flight24);
        var input = QualityInput.From(recording);
        var withoutRadioHeight = input with
        {
            Numbers = input.Numbers.Where(pair => pair.Key != "RADIO_HEIGHT_FT").ToDictionary(),
            Channels = input.Channels.Where(channel => channel.ParameterKey != "RADIO_HEIGHT_FT").ToList()
        };

        var result = QualityEvaluator.Evaluate(QualityParity.SeedRules, withoutRadioHeight);

        Assert.Contains(result.Findings, finding => finding is { Code: QualityEvaluator.InputMissing, ParameterKey: "RADIO_HEIGHT_FT" });
        Assert.Contains(result.Findings, finding => finding is { Code: QualityEvaluator.RequiredChannelCoverage, ParameterKey: "RADIO_HEIGHT_FT", Severity: ValidationSeverity.BLOCKING });
        Assert.Null(result.PartialStart);
        Assert.Null(result.FirstRadioHeightAirborneIndex);
        Assert.Equal(QualityGrade.NOT_USABLE, result.Grade);
    }

    private static void AssertGrade(
        QualityGrade expected,
        string[] reasons,
        QualitySpan[]? spans = null,
        int[]? gaps = null,
        ChannelCoverage? coverage = null,
        bool? partialStart = false,
        bool? partialEnd = false,
        bool blocking = false)
    {
        var decision = QualityGrader.Grade(QualityParity.SeedRules, blocking, [coverage ?? Full], new List<ValidationFinding>(), spans ?? [], gaps ?? [], partialStart, partialEnd);

        Assert.Equal(expected, decision.Grade);
        Assert.Equal(reasons, decision.Reasons);
    }

    private static QualitySpan Span(string code) => new(code, 0, 10, [], [], null, null, null, null, null, [], null);
}
