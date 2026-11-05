using System.Text.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.ConformanceTests.Quality;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public static class PhaseParity
{
    public const string Liftoff = "LIFTOFF";
    public const string Touchdown = "TOUCHDOWN";

    public static string SeedContent => ConfigurationSeeds.ContentOf(ConfigurationKind.PHASE_RULESET);

    public static PhaseRuleSetContent SeedRules { get; } = Validated(SeedContent);

    public static PhaseRuleSetContent Validated(string content)
    {
        var validation = PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog);

        Assert.True(validation.IsValid, string.Join("\n", validation.Issues.Select(issue => $"{issue.Path} {issue.Code} {issue.Message}")));

        return validation.Rules!;
    }

    public static (QualityInput Input, QualityResult Quality) QualityOf(string fileName, byte[] content)
    {
        var input = QualityInput.From(Recordings.Parse(fileName, content));

        return (input, QualityEvaluator.Evaluate(QualityParity.OracleRules, input));
    }

    public static PhaseResult Evaluate(string fileName, byte[] content) => Evaluate(SeedRules, fileName, content);

    public static PhaseResult Evaluate(PhaseRuleSetContent rules, string fileName, byte[] content)
    {
        var (input, quality) = QualityOf(fileName, content);

        return PhaseEvaluator.Evaluate(rules, input, quality);
    }

    public static PhaseEvaluation Run(string fileName, byte[] content)
    {
        var (input, quality) = QualityOf(fileName, content);

        return PhaseEvaluator.Run(SeedRules, input, quality)!;
    }

    public static void AssertMatchesOracle(JsonElement oracle, PhaseResult result, string fileName)
    {
        var liftoff = result.AnchorOf(Liftoff)!;
        var touchdown = result.AnchorOf(Touchdown)!;

        Assert.True(NullableInt(oracle, "liftoffIndex") == liftoff.Index, $"{fileName}: lift-off index");
        Assert.True(oracle.GetProperty("liftoffBasis").GetString() == liftoff.Basis.ToString(), $"{fileName}: lift-off basis");
        Assert.True(NullableInt(oracle, "touchdownIndex") == touchdown.Index, $"{fileName}: touchdown index");
        Assert.True(oracle.GetProperty("touchdownBasis").GetString() == touchdown.Basis.ToString(), $"{fileName}: touchdown basis");

        var expectedCoverage = oracle.GetProperty("coverage").EnumerateObject()
            .Select(pair => $"{pair.Name}={pair.Value.GetInt32()}")
            .Order(StringComparer.Ordinal);
        var actualCoverage = result.Coverage
            .Select(entry => $"{entry.PhaseCode}={entry.Samples}")
            .Order(StringComparer.Ordinal);
        Assert.True(expectedCoverage.SequenceEqual(actualCoverage), $"{fileName}: coverage");

        var expectedSegments = oracle.GetProperty("segments").EnumerateArray().Select(OracleSegmentKey).ToList();
        var actualSegments = result.Segments.Select(SegmentKey).ToList();
        var firstDifference = expectedSegments.Zip(actualSegments).Select((pair, position) => (pair, position)).FirstOrDefault(item => item.pair.First != item.pair.Second);

        Assert.True(expectedSegments.Count == actualSegments.Count, $"{fileName}: {expectedSegments.Count} segments expected, {actualSegments.Count} found");
        Assert.True(expectedSegments.SequenceEqual(actualSegments), $"{fileName}: segment {firstDifference.position}\n expected {firstDifference.pair.First}\n actual   {firstDifference.pair.Second}");
    }

    private static string OracleSegmentKey(JsonElement segment)
        => string.Join(" ",
            segment.GetProperty("phaseCode").GetString(),
            segment.GetProperty("startIndex").GetInt32(),
            segment.GetProperty("endIndexInclusive").GetInt32(),
            segment.GetProperty("startElapsed").GetDouble(),
            segment.GetProperty("endElapsed").GetDouble(),
            segment.GetProperty("confidence").GetString(),
            Joined(segment.GetProperty("evidenceCodes")),
            Joined(segment.GetProperty("flags")),
            Joined(segment.GetProperty("unknownReasons")));

    private static string SegmentKey(PhaseSegment segment)
        => string.Join(" ",
            segment.PhaseCode,
            segment.StartIndex,
            segment.EndIndexInclusive,
            segment.StartElapsed,
            segment.EndElapsed,
            segment.Confidence,
            $"[{string.Join(",", segment.EvidenceCodes)}]",
            $"[{string.Join(",", segment.Flags)}]",
            $"[{string.Join(",", segment.UnknownReasons)}]");

    private static string Joined(JsonElement array) => $"[{string.Join(",", array.EnumerateArray().Select(item => item.GetString()))}]";

    private static int? NullableInt(JsonElement element, string property)
        => element.GetProperty(property).ValueKind == JsonValueKind.Null ? null : element.GetProperty(property).GetInt32();
}
