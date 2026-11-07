using AeroTech.ECTM.Domain.ConformanceTests.Fakes;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Baselines;

public sealed class BaselineLifecycleTests
{
    [Fact]
    public void One_manual_value_is_enough_to_publish()
    {
        var baseline = TestBaselines.Draft(TestBaselines.Parameter(675));

        baseline.Publish(baseline.DraftRevision, "engineer", TestBaselines.Now);

        Assert.Equal(BaselineState.PUBLISHED, baseline.State);
        Assert.Equal(BaselineInputProvenance.MANUAL, baseline.InputProvenance);
    }

    [Fact]
    public void A_draft_may_hold_an_empty_row_but_cannot_be_published_with_it()
    {
        var baseline = TestBaselines.Draft(TestBaselines.Parameter(675), TestBaselines.Parameter(null, "N2_PCT", "PERCENT"));

        Assert.Equal([BaselineIssue.VALUE_MISSING], baseline.PublicationIssues());

        var refusal = Assert.Throws<BusinessException>(() => baseline.Publish(baseline.DraftRevision, "engineer", TestBaselines.Now));
        Assert.Equal(80404, refusal.Code);
    }

    [Fact]
    public void A_file_value_keeps_its_suggestion_and_records_the_edit()
    {
        var baseline = TestBaselines.Draft(
            TestBaselines.Parameter(678, suggestedRawValue: 678),
            TestBaselines.Parameter(674, "N1_PCT", "PERCENT", suggestedRawValue: 675),
            TestBaselines.Parameter(97, "N2_PCT", "PERCENT"));

        var origins = baseline.Parameters.ToDictionary(parameter => parameter.ParameterCode, parameter => parameter.ValueOrigin);

        Assert.Equal(BaselineValueOrigin.FILE_SUGGESTED, origins["EGT_C"]);
        Assert.Equal(BaselineValueOrigin.FILE_EDITED, origins["N1_PCT"]);
        Assert.Equal(BaselineValueOrigin.MANUAL, origins["N2_PCT"]);
        Assert.Equal(675, baseline.Parameters.Single(parameter => parameter.ParameterCode == "N1_PCT").SuggestedRawValue);
        Assert.Equal(BaselineInputProvenance.MIXED, baseline.InputProvenance);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void A_value_that_is_not_a_finite_number_is_refused(double value)
    {
        var refusal = Assert.Throws<BusinessException>(() => TestBaselines.Draft(TestBaselines.Parameter(value)));

        Assert.Equal(80405, refusal.Code);
    }

    [Fact]
    public void The_same_measurement_twice_is_refused()
    {
        var refusal = Assert.Throws<BusinessException>(() => TestBaselines.Draft(TestBaselines.Parameter(675), TestBaselines.Parameter(676)));

        Assert.Equal(80405, refusal.Code);
    }

    [Fact]
    public void The_same_parameter_at_another_snapshot_is_a_separate_measurement()
    {
        var baseline = TestBaselines.Draft(
            TestBaselines.Parameter(675),
            TestBaselines.Parameter(870, kind: SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED, selector: null));

        Assert.Equal(2, baseline.Parameters.Count);
        Assert.Equal(BaselineStatistic.AT_PEAK_SAMPLE, baseline.Parameters.Single(parameter => parameter.SnapshotKind == SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED).Statistic);
    }

    [Fact]
    public void A_published_baseline_cannot_be_changed()
    {
        var baseline = TestBaselines.Published(TestBaselines.Parameter(675));

        var refusal = Assert.Throws<BusinessException>(() => baseline.Revise(baseline.DraftRevision, TestBaselines.Description(), [TestBaselines.Parameter(674)], new SequenceIds()));

        Assert.Equal(80401, refusal.Code);
    }

    [Fact]
    public void A_stale_draft_revision_is_refused()
    {
        var baseline = TestBaselines.Draft(TestBaselines.Parameter(675));
        baseline.Revise(1, TestBaselines.Description(), [TestBaselines.Parameter(674)], new SequenceIds());

        var refusal = Assert.Throws<BusinessException>(() => baseline.Revise(1, TestBaselines.Description(), [TestBaselines.Parameter(673)], new SequenceIds()));

        Assert.Equal(80403, refusal.Code);
        Assert.Equal(2, baseline.DraftRevision);
    }

    [Fact]
    public void A_clone_is_a_new_draft_of_the_same_lineage_and_leaves_the_source_unchanged()
    {
        var published = TestBaselines.Published(TestBaselines.Parameter(675));

        var clone = published.CloneAsDraft(99, 2, new SequenceIds(), "engineer", TestBaselines.Now);
        clone.Revise(clone.DraftRevision, TestBaselines.Description(), [TestBaselines.Parameter(674)], new SequenceIds());

        Assert.Equal((BaselineState.DRAFT, published.LineageId, 2, published.Id), (clone.State, clone.LineageId, clone.RevisionNumber, clone.PreviousBaselineId!.Value));
        Assert.Equal(675, published.Parameters.Single().RawValue);
        Assert.Equal(674, clone.Parameters.Single().RawValue);
    }

    [Fact]
    public void Only_a_published_baseline_can_be_retired()
    {
        var draft = TestBaselines.Draft(TestBaselines.Parameter(675));

        var refusal = Assert.Throws<BusinessException>(() => draft.Retire("superseded", "engineer", TestBaselines.Now));

        Assert.Equal(80402, refusal.Code);
    }
}
