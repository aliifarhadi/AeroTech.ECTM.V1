using System.Text.Json;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Baselines;

public sealed class IndependentArithmeticTests
{
    private const double Precision = 1e-9;

    private static readonly JsonElement Fixture = JsonDocument.Parse(File.ReadAllText(
        Path.Combine(RepositoryPaths.Tests(), "AeroTech.ECTM.Domain.ConformanceTests", "Baselines", "independent-p07-v2.json"))).RootElement;

    public static TheoryData<string> CaseIds()
    {
        var ids = new TheoryData<string>();

        foreach (var item in Fixture.GetProperty("cases").EnumerateArray())
            ids.Add(item.GetProperty("id").GetString()!);

        return ids;
    }

    [Theory]
    [MemberData(nameof(CaseIds))]
    public void Every_frozen_case_gives_its_hand_calculated_result(string caseId)
    {
        var item = CaseOf(caseId);
        var parameterCode = ParameterCodeOf(item);
        var baselineUnit = Text(item, "baselineUnit") ?? Text(item, "unit") ?? TestBaselines.Celsius;
        var observedUnit = Text(item, "observedUnit") ?? baselineUnit;
        var baselineSelector = Text(item, "baselineSelector") ?? TestBaselines.Selector;
        var observedSelector = Text(item, "observationSelector") ?? baselineSelector;
        var baselineEngineId = Number(item, "baselineEngineId") ?? TestBaselines.EngineId;
        var observedEngineId = Number(item, "observationEngineId") ?? baselineEngineId;

        var parameter = TestBaselines.Parameter(item.GetProperty("baselineRawValue").GetDouble(), parameterCode, baselineUnit, selector: baselineSelector);
        var baseline = TestBaselines.Published(parameter, engineId: baselineEngineId);
        var observed = TestBaselines.Observed(Value(item, "rawObserved"), parameterCode, observedUnit, observedEngineId, selector: observedSelector);

        var comparison = BaselineComparisonEvaluator.Evaluate(baseline, baseline.Parameters.Single(), observed, ComparisonTolerances.None);

        AssertDelta(Value(item, "expectedRawDelta"), comparison.RawDelta);
        Assert.Contains(ComparisonReason.NO_APPROVED_CORRECTION_MODEL, comparison.Reasons);

        if (Text(item, "status") is { } status)
            Assert.Equal(Enum.Parse<ComparisonStatus>(status), comparison.Status);

        if (Text(item, "reason") is { } reason)
            Assert.Equal(Enum.Parse<ComparisonReason>(reason), comparison.Reasons[0]);
    }

    private static JsonElement CaseOf(string caseId)
        => Fixture.GetProperty("cases").EnumerateArray().Single(item => item.GetProperty("id").GetString() == caseId);

    private static string ParameterCodeOf(JsonElement item)
    {
        var metric = Text(item, "metric") ?? "CRUISE/OPERATOR_60S/EGT_C/MEAN";

        return metric.Split('/')[2];
    }

    private static void AssertDelta(double? expected, double? actual)
    {
        if (expected is null)
        {
            Assert.Null(actual);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(expected.Value, actual.Value, Precision);
    }

    private static string? Text(JsonElement item, string name)
        => item.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static double? Value(JsonElement item, string name)
        => item.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : null;

    private static long? Number(JsonElement item, string name)
        => Text(item, name) is { } text ? long.Parse(text) : null;
}
