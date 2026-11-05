using System.Text.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public static class QualityParity
{
    public static QualityRulesContent SeedRules { get; } =
        QualityRulesValidator.Validate(ConfigurationSeeds.ContentOf(ConfigurationKind.QUALITY_RULES), SeedDocuments.ValidatedCatalog).Rules!;

    public static QualityRulesContent OracleRules { get; } = SeedRules with { Clock = SeedRules.Clock! with { AcceptOneDigitHour = false } };

    public static QualityResult Evaluate(string fileName, byte[] content)
        => QualityEvaluator.Evaluate(OracleRules, QualityInput.From(Recordings.Parse(fileName, content)));

    public static QualityResult EvaluateWithSeed(string fileName, byte[] content)
        => QualityEvaluator.Evaluate(SeedRules, QualityInput.From(Recordings.Parse(fileName, content)));

    public static void AssertMatchesOracle(JsonElement oracle, QualityResult result, string fileName)
    {
        Assert.True(oracle.GetProperty("sampleCount").GetInt32() == result.SampleCount, fileName);
        Assert.True(IntsOf(oracle.GetProperty("elapsedGaps")).SequenceEqual(result.ElapsedGaps), $"{fileName}: elapsed gaps");
        Assert.True(oracle.GetProperty("partialStart").GetBoolean() == result.PartialStart, $"{fileName}: partial start");
        Assert.True(oracle.GetProperty("partialEnd").GetBoolean() == result.PartialEnd, $"{fileName}: partial end");
        Assert.True(NullableInt(oracle, "airGroundLagSec") == result.AirGroundLagSec, $"{fileName}: lag");
        Assert.True(NullableInt(oracle, "firstRadioHeightAirborneIndex") == result.FirstRadioHeightAirborneIndex, $"{fileName}: first airborne");
        Assert.True(NullableInt(oracle, "firstAirFlagIndex") == result.FirstAirFlagIndex, $"{fileName}: first AIR");

        var expectedJumps = oracle.GetProperty("discontinuities").EnumerateArray()
            .Select(jump => (jump.GetProperty("index").GetInt32(), NullableDouble(jump, "dAltFtPerSec"), NullableDouble(jump, "dRhFtPerSec"), NullableDouble(jump, "radioHeightBeforeFt"), jump.GetProperty("code").GetString()))
            .ToList();
        var actualJumps = result.Discontinuities
            .Select(jump => (jump.Index, jump.DAltFtPerSec, jump.DRhFtPerSec, jump.RadioHeightBeforeFt, (string?)jump.Code))
            .ToList();
        Assert.True(expectedJumps.SequenceEqual(actualJumps), $"{fileName}: discontinuities");

        var expectedSpans = oracle.GetProperty("spans").EnumerateArray()
            .Select(span => SpanKey(span.GetProperty("code").GetString()!, span.GetProperty("startIndex").GetInt32(), span.GetProperty("endIndexInclusive").GetInt32(), IntsOf(span.GetProperty("channels")), EvidenceOf(span)))
            .ToList();
        var actualSpans = result.Spans
            .Select(span => SpanKey(span.Code, span.StartIndex, span.EndIndexInclusive, span.ProfileOrdinals, $"{span.JumpIndex}|{span.Cause}"))
            .ToList();
        Assert.True(expectedSpans.SequenceEqual(actualSpans), $"{fileName}: spans\n expected {string.Join("; ", expectedSpans)}\n actual   {string.Join("; ", actualSpans)}");

        var clock = oracle.GetProperty("clock");
        var expectedHistogram = clock.GetProperty("offsetHistogram").EnumerateObject().Select(pair => $"{pair.Name}={pair.Value.GetInt32()}");
        var actualHistogram = result.Clock!.OffsetHistogram.Select(pair => $"{pair.Key}={pair.Value}");

        Assert.True(clock.GetProperty("disagreeRows").GetInt32() == result.Clock.DisagreeRows, $"{fileName}: disagree");
        Assert.True(clock.GetProperty("invalidOrMissingRows").GetInt32() == result.Clock.InvalidOrMissingRows, $"{fileName}: invalid clocks");
        Assert.True(clock.GetProperty("primaryNotInStepRows").GetInt32() == result.Clock.PrimaryNotInStepRows, $"{fileName}: primary step");
        Assert.True(clock.GetProperty("secondaryNotInStepRows").GetInt32() == result.Clock.SecondaryNotInStepRows, $"{fileName}: secondary step");
        Assert.True(expectedHistogram.SequenceEqual(actualHistogram), $"{fileName}: offset histogram");
        Assert.True(Enum.Parse<TimeTrust>(clock.GetProperty("timeTrust").GetString()!) == result.Clock.TimeTrust, $"{fileName}: time trust");
    }

    private static string SpanKey(string code, int start, int end, IEnumerable<int> ordinals, string evidence)
        => $"{code} {start}-{end} [{string.Join(",", ordinals)}] {evidence}";

    private static string EvidenceOf(JsonElement span)
    {
        var evidence = span.GetProperty("evidence");
        var jumpIndex = evidence.TryGetProperty("jumpIndex", out var jump) ? jump.GetInt32().ToString() : "";
        var cause = evidence.TryGetProperty("cause", out var reason) ? reason.GetString() : "";

        return $"{jumpIndex}|{cause}";
    }

    private static List<int> IntsOf(JsonElement array) => array.EnumerateArray().Select(item => item.GetInt32()).ToList();

    private static int? NullableInt(JsonElement element, string property)
        => element.GetProperty(property).ValueKind == JsonValueKind.Null ? null : element.GetProperty(property).GetInt32();

    private static double? NullableDouble(JsonElement element, string property)
        => element.GetProperty(property).ValueKind == JsonValueKind.Null ? null : element.GetProperty(property).GetDouble();
}
