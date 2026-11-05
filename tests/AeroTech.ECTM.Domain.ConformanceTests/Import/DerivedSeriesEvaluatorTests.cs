using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class DerivedSeriesEvaluatorTests
{
    private const string EgtOverN2 = """
        {"code":"EGT_OVER_N2","kind":"RATIO","role":"ENGINE_PERFORMANCE","unitOut":"CELSIUS_PER_PERCENT","status":"CANDIDATE",
         "description":"EGT per percent N2","numerator":"EGT_C[pos]","denominator":"N2_PCT[pos]","guardZero":true}
        """;

    [Fact]
    public void A_difference_is_evaluated_at_every_sample_of_a_real_flight()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);

        var egtDifference = Evaluate(SeedDocuments.ValidatedCatalog, recording).Single(series => series.ParameterKey == "EGT_DIFF_E2_E1_C");

        Assert.Equal(recording.SampleCount, egtDifference.AvailableCount);
        Assert.Equal(873 - 848, egtDifference.Values[101]);
        Assert.Equal(872 - 861, egtDifference.Values[121]);
        Assert.StartsWith("EGT_C[2] - EGT_C[1] = ", egtDifference.FirstAvailableFormula);
    }

    [Fact]
    public void A_derived_parameter_can_use_an_earlier_derived_parameter()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var series = Evaluate(SeedDocuments.ValidatedCatalog, recording);
        var tat = Recordings.Channel(recording, "TAT_C").Numbers![101]!.Value;

        var theta = series.Single(candidate => candidate.ParameterKey == "THETA");

        Assert.Equal((273.15 + 1.0 * tat) / 288.15, theta.Values[101]);
        Assert.Equal(recording.SampleCount, theta.AvailableCount);
    }

    [Fact]
    public void A_position_definition_gives_one_series_per_engine()
    {
        var catalogContent = SeedDocuments.Changed(SeedDocuments.Catalog, catalog => catalog["derivedParameters"]!.AsArray().Add(JsonNode.Parse(EgtOverN2)));
        var catalog = ParameterCatalogValidator.Validate(catalogContent).Catalog!;
        var recording = Recordings.ParseReal(Recordings.Flight25);

        var series = Evaluate(catalog, recording).Where(candidate => candidate.Code == "EGT_OVER_N2").ToList();

        Assert.Equal(["EGT_OVER_N2[1]", "EGT_OVER_N2[2]"], series.Select(candidate => candidate.ParameterKey));
        Assert.Equal(848 / 97.85, series[0].Values[101]);
        Assert.Equal(873 / 97.87, series[1].Values[101]);
    }

    [Fact]
    public void A_missing_input_channel_gives_no_value_and_says_why()
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);
        var withoutEgtOne = recording.Channels.Where(channel => channel.ParameterKey != "EGT_C[1]").ToList();

        var egtDifference = DerivedSeriesEvaluator.Evaluate(SeedDocuments.ValidatedCatalog, withoutEgtOne, recording.SampleCount)
            .Single(series => series.ParameterKey == "EGT_DIFF_E2_E1_C");

        Assert.All(egtDifference.Values, value => Assert.Null(value));
        Assert.Equal(recording.SampleCount, egtDifference.StatusCounts[DerivedParameterEvaluator.MissingInput]);
        Assert.Null(egtDifference.FirstAvailableSample);
    }

    [Fact]
    public void Findings_are_compared_by_code_and_parameter()
    {
        ValidationFinding[] stored =
        [
            new(ValidationSeverity.INFO, ValidationCodes.ChannelCoverage, "a", "AT_N1_LIMIT_MODE", 10, 0),
            new(ValidationSeverity.INFO, ValidationCodes.PlausibilityOutOfRange, "b", "N2_PCT[1]", 5, 1),
            new(ValidationSeverity.WARNING, ValidationCodes.TimeAxisGap, "c", "ELAPSED_S", 1, 3)
        ];
        ValidationFinding[] dryRun =
        [
            new(ValidationSeverity.INFO, ValidationCodes.ChannelCoverage, "a", "AT_N1_LIMIT_MODE", 10, 0),
            new(ValidationSeverity.INFO, ValidationCodes.PlausibilityOutOfRange, "b", "N2_PCT[1]", 7, 1.4),
            new(ValidationSeverity.INFO, ValidationCodes.PlausibilityOutOfRange, "d", "N1_PCT[1]", 2, 0.1)
        ];

        var changes = FindingComparison.Compare(stored, dryRun).ToDictionary(change => change.ParameterKey!, change => change.Change);

        Assert.Equal(FindingComparison.Unchanged, changes["AT_N1_LIMIT_MODE"]);
        Assert.Equal(FindingComparison.Changed, changes["N2_PCT[1]"]);
        Assert.Equal(FindingComparison.Added, changes["N1_PCT[1]"]);
        Assert.Equal(FindingComparison.Removed, changes["ELAPSED_S"]);
    }

    private static IReadOnlyList<DerivedSeries> Evaluate(ValidatedCatalog catalog, ParsedRecording recording)
        => DerivedSeriesEvaluator.Evaluate(catalog, recording.Channels, recording.SampleCount);
}
