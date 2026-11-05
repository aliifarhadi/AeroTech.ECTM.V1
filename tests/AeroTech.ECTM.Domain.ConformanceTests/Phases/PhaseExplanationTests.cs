using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public sealed class PhaseExplanationTests
{
    [Theory]
    [MemberData(nameof(PhaseGoldenTests.RealFiles), MemberType = typeof(PhaseGoldenTests))]
    public void Every_transition_is_explained_by_values_that_satisfy_the_fired_rule(string fileName)
    {
        var evaluation = PhaseParity.Run(fileName, Recordings.RealBytes(fileName));

        foreach (var segment in evaluation.Segments)
        {
            var explanation = evaluation.Explain(segment.StartIndex);
            var fired = explanation.Rules.Where(rule => rule.Fired).ToList();
            var context = $"{fileName} sample {segment.StartIndex}";

            Assert.True(fired.Count <= 1, context);
            Assert.Equal(explanation.FiredRuleId, fired.SingleOrDefault()?.RuleId);
            Assert.All(explanation.Rules.TakeWhile(rule => !rule.Fired), rule => Assert.False(rule.Matched, $"{context}: {rule.RuleId} is above the fired rule"));
            Assert.All(explanation.Rules, rule => AssertConsistent(rule.When, context));

            if (explanation.ChangedBy is null)
                Assert.Equal(fired.SingleOrDefault()?.Label ?? PhaseParity.SeedRules.Fallback!.Label, explanation.Label);
            else
                Assert.Equal(FlightPhaseCode.UNKNOWN, explanation.Label);
        }
    }

    [Fact]
    public void The_stored_transitions_carry_the_trace_of_the_fired_rule()
    {
        var result = PhaseParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));

        Assert.Equal(result.Segments.Count, result.Transitions.Count);
        Assert.All(result.Transitions.Where(transition => transition.FiredRuleId is not null), transition => Assert.True(transition.When!.Holds));
        Assert.Null(result.Transitions[0].FromPhase);
    }

    [Theory]
    [InlineData(Recordings.Flight24, 0)]
    [InlineData("2026-09-24_EP-DOA_9072.csv", 213 + 184)]
    public void A_draft_with_a_cruise_minimum_of_240_s_turns_only_shorter_cruises_into_level(string fileName, int expectedChanged)
    {
        var bytes = Recordings.RealBytes(fileName);

        var current = PhaseParity.Evaluate(fileName, bytes);
        var proposed = PhaseParity.Evaluate(PhaseParity.Validated(CruiseMinimumDraft(240)), fileName, bytes);
        var diff = PhaseComparison.Compare(current, proposed);

        Assert.Equal(expectedChanged, diff.ChangedSamples);
        Assert.Equal(CruiseSamples(current) - CruiseSamples(proposed), diff.ChangedSamples);
        Assert.All(diff.Changes, change => Assert.Equal((FlightPhaseCode.CRUISE, FlightPhaseCode.LEVEL), (change.Before!.Value, change.After!.Value)));
        Assert.Empty(diff.Anchors);
    }

    public static string CruiseMinimumDraft(int seconds)
        => SeedDocuments.Changed(PhaseParity.SeedContent, document =>
        {
            var cruise = document["conditions"]!.AsArray().Single(condition => condition!["code"]!.GetValue<string>() == "CRUISE")!;
            cruise["minSec"] = seconds;
        });

    [Fact]
    public void The_same_rules_give_the_same_result()
    {
        var bytes = Recordings.RealBytes(Recordings.Flight24);

        var first = PhaseParity.Evaluate(Recordings.Flight24, bytes);
        var second = PhaseParity.Evaluate(Recordings.Flight24, bytes);

        Assert.Equal(first.ToJson(), second.ToJson());
        Assert.Equal(0, PhaseComparison.Compare(first, second).ChangedSamples);
    }

    [Fact]
    public void A_stored_result_reads_back_unchanged()
    {
        var result = PhaseParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));

        Assert.Equal(result.ToJson(), PhaseResult.FromJson(result.ToJson()).ToJson());
    }

    private static int CruiseSamples(PhaseResult result)
        => result.Coverage.SingleOrDefault(entry => entry.PhaseCode == FlightPhaseCode.CRUISE)?.Samples ?? 0;

    private static void AssertConsistent(PredicateTrace trace, string context)
    {
        var expected = trace.Operator switch
        {
            ComparePredicate.Operator => ComparedHolds(trace, trace.Observed),
            AbsComparePredicate.Operator => ComparedHolds(trace, trace.Observed is { } value ? Math.Abs(value) : null),
            EqualsPredicate.Operator => trace.ObservedText == trace.Expected,
            AllPredicate.Operator => trace.Of!.All(child => child.Holds),
            AnyPredicate.Operator => trace.Of!.Any(child => child.Holds),
            NotPredicate.Operator => !trace.Of!.Single().Holds,
            InConditionPredicate.Operator => trace.Condition!.Contains,
            _ => trace.Holds
        };

        Assert.True(expected == trace.Holds, $"{context}: {trace.Operator} {trace.Channel ?? trace.Reference} recorded {trace.Holds}");

        if (trace.Condition is { Operator: Sustained.Operator } sustained)
        {
            var qualifies = sustained.Predicate!.Holds && sustained.RunSeconds >= sustained.RequiredSec;
            Assert.True(qualifies == sustained.Contains, $"{context}: {sustained.Code} run {sustained.RunSeconds} s of {sustained.RequiredSec} s");
        }

        foreach (var child in trace.Of ?? [])
            AssertConsistent(child, context);

        if (trace.Condition?.Predicate is { } inner)
            AssertConsistent(inner, context);
    }

    private static bool ComparedHolds(PredicateTrace trace, double? observed)
        => observed is { } value ? Comparison.Holds(value, trace.Op!.Value, trace.Threshold!.Value) : trace.NullAllowed == true;
}
