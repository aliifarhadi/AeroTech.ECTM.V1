using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Baselines;

public sealed class ComparisonWarningTests
{
    private static readonly ComparisonTolerances CandidateTolerances = new(31,
    [
        new BaselineConditionRule("PRESSURE_ALT_FT", false, 2000),
        new BaselineConditionRule("MACH", false, 0.03),
        new BaselineConditionRule("TAT_C", false, 8),
        new BaselineConditionRule("N1_PCT", true, 1.5)
    ]);

    [Fact]
    public void A_condition_outside_its_tolerance_is_a_warning_and_the_raw_difference_stays()
    {
        var parameter = TestBaselines.Parameter(675, referenceContext: TestBaselines.Context(29227, 0.758, -6.31, 84.65));
        var observed = TestBaselines.Observed(681, context: TestBaselines.Context(31012.6, 0.76, -14.92, 85.89));

        var comparison = Compare(parameter, observed, CandidateTolerances);

        Assert.Equal(ComparisonStatus.DESCRIPTIVE_RAW, comparison.Status);
        Assert.Equal(6, comparison.RawDelta);
        Assert.Contains(ComparisonReason.CONDITION_OUT_OF_TOLERANCE, comparison.Reasons);

        var tat = comparison.Detail.Conditions.Single(check => check.ParameterCode == "TAT_C");
        Assert.Equal((-8.61, 8.0, true), (Math.Round(tat.Difference!.Value, 2), tat.Tolerance!.Value, tat.OutsideTolerance!.Value));

        var n1 = comparison.Detail.Conditions.Single(check => check.ParameterCode == "N1_PCT");
        Assert.Equal((true, false), (n1.PerEngine, n1.OutsideTolerance!.Value));
    }

    [Fact]
    public void Without_baseline_rules_the_differences_are_shown_without_a_warning()
    {
        var parameter = TestBaselines.Parameter(675, referenceContext: TestBaselines.Context(29227, 0.758, -6.31, 84.65));
        var observed = TestBaselines.Observed(681, context: TestBaselines.Context(31012.6, 0.76, -14.92, 85.89));

        var comparison = Compare(parameter, observed, ComparisonTolerances.None);

        Assert.DoesNotContain(ComparisonReason.CONDITION_OUT_OF_TOLERANCE, comparison.Reasons);
        Assert.All(comparison.Detail.Conditions, check => Assert.Null(check.OutsideTolerance));
        Assert.Equal(3, comparison.Detail.Conditions.Count);
    }

    [Fact]
    public void A_manual_value_without_reference_conditions_is_context_unknown()
    {
        var comparison = Compare(TestBaselines.Parameter(675), TestBaselines.Observed(681, context: TestBaselines.Context(31000, 0.76, -11, 85.9)), CandidateTolerances);

        Assert.Equal(6, comparison.RawDelta);
        Assert.Contains(ComparisonReason.CONTEXT_UNKNOWN, comparison.Reasons);
    }

    [Fact]
    public void An_unverified_takeoff_context_keeps_the_value_with_its_warning()
    {
        var parameter = TestBaselines.Parameter(890, kind: SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT, selector: null);
        var observed = TestBaselines.Observed(896, kind: SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT, selector: null, contextStatus: SnapshotContextStatus.CONTEXT_UNVERIFIED_CHANNEL_COHERENCE);

        var comparison = Compare(parameter, observed, ComparisonTolerances.None);

        Assert.Equal(6, comparison.RawDelta);
        Assert.Contains(ComparisonReason.CONTEXT_UNVERIFIED_CHANNEL_COHERENCE, comparison.Reasons);
    }

    [Fact]
    public void A_different_configuration_is_a_warning()
    {
        var parameter = TestBaselines.Parameter(675, referenceContext: TestBaselines.Context(29227, 0.758, -6.31, 84.65));
        var observed = TestBaselines.Observed(681, context: TestBaselines.Context(29300, 0.76, -6, 84.7, "On|On|On|On|On|On|On"));

        var comparison = Compare(parameter, observed, CandidateTolerances);

        Assert.Contains(ComparisonReason.CONFIGURATION_DIFFERS, comparison.Reasons);
        Assert.DoesNotContain(ComparisonReason.CONDITION_OUT_OF_TOLERANCE, comparison.Reasons);
    }

    [Fact]
    public void The_reference_file_itself_is_marked_and_still_compared_with_the_edited_value()
    {
        var sha256 = new string('a', 64);
        var baseline = TestBaselines.Published(TestBaselines.Parameter(674, suggestedRawValue: 675), sourceFileSha256: sha256);
        var observed = TestBaselines.Observed(675, sourceFileSha256: sha256);

        var comparison = BaselineComparisonEvaluator.Evaluate(baseline, baseline.Parameters.Single(), observed, ComparisonTolerances.None);

        Assert.Equal(1, comparison.RawDelta);
        Assert.Contains(ComparisonReason.REFERENCE_SOURCE_POINT, comparison.Reasons);
    }

    [Theory]
    [InlineData(-3, ComparisonReason.RETROSPECTIVE)]
    [InlineData(0, ComparisonReason.TEMPORAL_ORDER_UNVERIFIED)]
    public void A_flight_before_or_on_the_declared_date_is_labelled(int daysAfterEffectiveDate, ComparisonReason expected)
    {
        var observed = TestBaselines.Observed(681, flightDate: TestBaselines.EffectiveDate.AddDays(daysAfterEffectiveDate));

        var comparison = Compare(TestBaselines.Parameter(675), observed, ComparisonTolerances.None);

        Assert.Contains(expected, comparison.Reasons);
        Assert.Equal(6, comparison.RawDelta);
    }

    [Fact]
    public void A_later_flight_has_no_chronology_label()
    {
        var comparison = Compare(TestBaselines.Parameter(675), TestBaselines.Observed(681), ComparisonTolerances.None);

        Assert.DoesNotContain(ComparisonReason.RETROSPECTIVE, comparison.Reasons);
        Assert.DoesNotContain(ComparisonReason.TEMPORAL_ORDER_UNVERIFIED, comparison.Reasons);
    }

    [Fact]
    public void Another_snapshot_rule_set_keeps_the_raw_difference_with_a_warning()
    {
        var comparison = Compare(TestBaselines.Parameter(675), TestBaselines.Observed(681, snapshotRulesDocumentId: 16), ComparisonTolerances.None);

        Assert.Equal(6, comparison.RawDelta);
        Assert.Contains(ComparisonReason.SNAPSHOT_RULESET_CHANGED, comparison.Reasons);
    }

    [Fact]
    public void A_missing_observation_never_becomes_zero()
    {
        var comparison = Compare(TestBaselines.Parameter(675), TestBaselines.Observed(null), CandidateTolerances);

        Assert.Equal(ComparisonStatus.NOT_COMPARABLE, comparison.Status);
        Assert.Null(comparison.RawDelta);
        Assert.Equal(ComparisonReason.MISSING_OBSERVATION, comparison.Reasons[0]);
    }

    private static BaselineComparison Compare(AeroTech.ECTM.Domain.BaselineAggregate.Arguments.BaselineParameterArgs parameter, ObservedMeasurement observed, ComparisonTolerances tolerances)
    {
        var baseline = TestBaselines.Published(parameter);

        return BaselineComparisonEvaluator.Evaluate(baseline, baseline.Parameters.Single(), observed, tolerances);
    }
}
