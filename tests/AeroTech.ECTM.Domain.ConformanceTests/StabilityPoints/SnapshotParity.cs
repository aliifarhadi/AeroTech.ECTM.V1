using System.Globalization;
using System.Text.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.ConformanceTests.Phases;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.StabilityPoints;

public static class SnapshotParity
{
    public static readonly IReadOnlyDictionary<string, string> OracleEngineKeys = new Dictionary<string, string>
    {
        ["n1"] = "N1_PCT",
        ["n2"] = "N2_PCT",
        ["egt"] = "EGT_C",
        ["ff"] = "FUEL_FLOW_PPH",
        ["oilp"] = "OIL_PRESSURE_PSI",
        ["oilt"] = "OIL_TEMP_C",
        ["fanvib"] = "VIB_N1_FAN",
        ["hpcvib"] = "VIB_N2_HPC",
        ["lptvib"] = "VIB_N1_LPT",
        ["hptvib"] = "VIB_N2_HPT"
    };

    public static string SeedContent => ConfigurationSeeds.ContentOf(ConfigurationKind.SNAPSHOT_RULES);

    public static SnapshotRulesContent SeedRules { get; } = Validated(SeedContent);

    public static SnapshotRulesContent Validated(string content)
    {
        var validation = SnapshotRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog);

        Assert.True(validation.IsValid, string.Join("\n", validation.Issues.Select(issue => $"{issue.Path} {issue.Code} {issue.Message}")));

        return validation.Rules!;
    }

    public static StabilityPointsEvaluation Evaluate(string fileName, byte[] content) => Evaluate(SeedRules, fileName, content);

    public static StabilityPointsEvaluation Evaluate(SnapshotRulesContent rules, string fileName, byte[] content)
    {
        var (input, quality) = PhaseParity.QualityOf(fileName, content);
        var phases = PhaseEvaluator.Evaluate(PhaseParity.SeedRules, input, quality);

        return StabilityPointsEvaluator.Evaluate(rules, input, quality, phases);
    }

    public static void AssertMatchesOracle(JsonElement oracle, StabilityPointsResult result, string fileName)
    {
        foreach (var position in new[] { 1, 2 })
        {
            var expected = TakeoffKey(oracle.GetProperty("takeoff").GetProperty(position.ToString()), position);
            var actual = TakeoffKey(result, position);

            AssertSame(expected, actual, $"{fileName} takeoff engine {position}");
        }

        foreach (var selector in oracle.GetProperty("cruise").EnumerateObject())
        {
            var actualSelector = result.Cruise.Single(entry => entry.Selector == selector.Name);

            foreach (var position in new[] { 1, 2 })
            {
                var expected = CruiseKey(selector.Value.GetProperty("engines").GetProperty(position.ToString()), position);
                var actual = CruiseKey(actualSelector.Engines.Single(engine => engine.Position == position));

                AssertSame(expected, actual, $"{fileName} cruise {selector.Name} engine {position}");
            }

            var expectedAvailability = selector.Value.GetProperty("availability").GetString();
            var actualAvailability = actualSelector.AvailableEngineCount > 0 ? "AVAILABLE" : "NOT_EVALUABLE";

            Assert.True(expectedAvailability == actualAvailability, $"{fileName} cruise {selector.Name} availability");
        }
    }

    private static List<string> TakeoffKey(JsonElement oracle, int position)
    {
        var key = new List<string> { $"availability={oracle.GetProperty("availability").GetString()}" };

        if (oracle.TryGetProperty("reason", out var reason))
            key.Add($"reason={reason.GetString()}");

        if (oracle.GetProperty("availability").GetString() != "AVAILABLE")
            return key;

        key.Add($"basis={oracle.GetProperty("searchBasis").GetString()}");
        key.Add($"peak={oracle.GetProperty("peakIndex").GetInt32()}");
        key.Add($"window={oracle.GetProperty("startIndex").GetInt32()}-{oracle.GetProperty("endIndexInclusive").GetInt32()}");
        key.Add($"valid={oracle.GetProperty("validCount").GetInt32()}/{oracle.GetProperty("validEgtCount").GetInt32()}");
        key.Add($"n1Peak={Number(oracle.GetProperty("n1Peak"))}");
        key.Add($"egtAtPeak={Number(oracle.GetProperty("egtAtPeak"))}");
        key.Add($"maxEgt={Number(oracle.GetProperty("maxEgtInSearchWindow"))}");
        key.AddRange(MeansOf(oracle.GetProperty("windowMeans"), position));

        var context = oracle.GetProperty("context");
        key.Add($"TAT_C={Number(context.GetProperty("tat"))}");
        key.Add($"PRESSURE_ALT_FT={Number(context.GetProperty("altitude"))}");
        key.Add($"MACH={Number(context.GetProperty("mach"))}");
        key.Add($"toga=[{string.Join(",", context.GetProperty("toga").EnumerateArray().Select(item => item.GetString()))}]");
        key.Add($"configuration=[{string.Join(",", context.GetProperty("configuration").EnumerateArray().Select(item => item.GetString()))}]");
        key.Add($"context={oracle.GetProperty("contextStatus").GetString()}");

        return key;
    }

    private static List<string> TakeoffKey(StabilityPointsResult result, int position)
    {
        var n1Peak = result.TakeoffN1Peaks.Single(peak => peak.Position == position);
        var egtPeak = result.TakeoffEgtPeaks.Single(peak => peak.Position == position);
        var key = new List<string> { $"availability={n1Peak.Availability}" };

        if (n1Peak.Reason is { } reason)
            key.Add($"reason={reason}");

        if (n1Peak.Availability != SnapshotAvailability.AVAILABLE)
            return key;

        key.Add($"basis={n1Peak.SearchBasis}");
        key.Add($"peak={n1Peak.PeakIndex}");
        key.Add($"window={n1Peak.StartIndex}-{n1Peak.EndIndexInclusive}");
        key.Add($"valid={n1Peak.ValidCount}/{n1Peak.ValidEgtCount}");
        key.Add($"n1Peak={Number(n1Peak.N1Peak)}");
        key.Add($"egtAtPeak={Number(n1Peak.EgtAtPeak)}");
        key.Add($"maxEgt={Number(egtPeak.EgtPeak)}");
        key.AddRange(MeansOf(n1Peak.Values));
        key.Add(ConditionMean(n1Peak.Context!, "TAT_C"));
        key.Add(ConditionMean(n1Peak.Context!, "PRESSURE_ALT_FT"));
        key.Add(ConditionMean(n1Peak.Context!, "MACH"));
        key.Add($"toga=[{string.Join(",", n1Peak.Context!.Toga)}]");
        key.Add($"configuration=[{string.Join(",", n1Peak.Context.Configurations)}]");
        key.Add($"context={n1Peak.ContextStatus}");

        return key;
    }

    private static List<string> CruiseKey(JsonElement oracle, int position)
    {
        var key = new List<string> { $"availability={oracle.GetProperty("availability").GetString()}" };

        if (oracle.TryGetProperty("reason", out var reason))
            key.Add($"reason={reason.GetString()}");

        if (oracle.TryGetProperty("blockStartIndex", out var blockStart))
            key.Add($"block={blockStart.GetInt32()}-{oracle.GetProperty("blockEndIndexInclusive").GetInt32()} {Number(oracle.GetProperty("blockSeconds"))}s");

        if (oracle.GetProperty("availability").GetString() != "AVAILABLE")
            return key;

        key.Add($"report={oracle.GetProperty("reportStartIndex").GetInt32()}-{oracle.GetProperty("reportEndIndexInclusive").GetInt32()}");
        key.AddRange(MeansOf(oracle.GetProperty("windowMeans"), position));

        var context = oracle.GetProperty("context");
        key.Add($"PRESSURE_ALT_FT={Number(context.GetProperty("altitude"))}");
        key.Add($"MACH={Number(context.GetProperty("mach"))}");
        key.Add($"TAT_C={Number(context.GetProperty("tat"))}");
        key.Add($"configuration=[{context.GetProperty("configuration").GetString()}]");

        return key;
    }

    private static List<string> CruiseKey(CruiseSnapshot snapshot)
    {
        var key = new List<string> { $"availability={snapshot.Availability}" };

        if (snapshot.Reason is { } reason)
            key.Add($"reason={reason}");

        if (snapshot.HasBlock)
            key.Add($"block={snapshot.BlockStartIndex}-{snapshot.BlockEndIndexInclusive} {Number(snapshot.BlockSeconds)}s");

        if (snapshot.Availability != SnapshotAvailability.AVAILABLE)
            return key;

        key.Add($"report={snapshot.ReportStartIndex}-{snapshot.ReportEndIndexInclusive}");
        key.AddRange(MeansOf(snapshot.Values));
        key.Add(ConditionMean(snapshot.Context!, "PRESSURE_ALT_FT"));
        key.Add(ConditionMean(snapshot.Context!, "MACH"));
        key.Add(ConditionMean(snapshot.Context!, "TAT_C"));
        key.Add($"configuration=[{string.Join(",", snapshot.Context!.Configurations)}]");

        return key;
    }

    private static IEnumerable<string> MeansOf(JsonElement means, int position)
        => OracleEngineKeys.Select(pair => $"{pair.Value}[{position}]={Number(means.GetProperty(pair.Key))}");

    private static IEnumerable<string> MeansOf(IReadOnlyList<ParameterSummary> values)
        => OracleEngineKeys.Values.Select(code => values.Single(value => value.ParameterKey.StartsWith(code + "[", StringComparison.Ordinal)))
            .Select(value => $"{value.ParameterKey}={Number(value.Mean)}");

    private static string ConditionMean(SnapshotContext context, string key)
        => $"{key}={Number(context.Conditions.Single(condition => condition.ParameterKey == key).Mean)}";

    private static string Number(JsonElement value)
        => value.ValueKind == JsonValueKind.Null ? "null" : Number(value.GetDouble());

    private static string Number(double? value)
        => value is { } present ? present.ToString("R", CultureInfo.InvariantCulture) : "null";

    private static void AssertSame(IReadOnlyList<string> expected, IReadOnlyList<string> actual, string context)
    {
        var difference = expected.Zip(actual).FirstOrDefault(pair => pair.First != pair.Second);

        Assert.True(expected.SequenceEqual(actual), $"{context}\n expected {difference.First ?? string.Join(" ", expected)}\n actual   {difference.Second ?? string.Join(" ", actual)}");
    }
}
