using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public sealed class QualityRulesValidatorTests
{
    private static string Seed => ConfigurationSeeds.ContentOf(ConfigurationKind.QUALITY_RULES);

    [Fact]
    public void The_seed_is_valid_against_the_catalog_seed()
    {
        var validation = QualityRulesValidator.Validate(Seed, SeedDocuments.ValidatedCatalog);

        Assert.Empty(validation.Issues);
        Assert.Equal("CANDIDATE", validation.Rules!.CalibrationStatus);
    }

    [Theory]
    [InlineData("$.coverage.blockingBelowPct", QualityRulesValidator.ValueInvalid)]
    [InlineData("$.channels.altitude", QualityRulesValidator.ChannelKindInvalid)]
    [InlineData("$.channels.engineN1", QualityRulesValidator.ChannelUnknown)]
    [InlineData("$.coverage.requiredChannels[0]", QualityRulesValidator.ValueInvalid)]
    [InlineData("$.affects.NOT_A_SPAN", QualityRulesValidator.SpanCodeUnknown)]
    [InlineData("$.clock", QualityRulesValidator.FieldRequired)]
    [InlineData("$.grade.masksAllowedForA[0].cause", QualityRulesValidator.ValueInvalid)]
    [InlineData("$.stuckSensor.channels[2].minN1ChangePct", QualityRulesValidator.ValueInvalid)]
    public void A_broken_document_names_the_path_of_the_problem(string path, string code)
    {
        var content = SeedDocuments.Changed(Seed, rules =>
        {
            rules["coverage"]!["blockingBelowPct"] = 99;
            rules["channels"]!["altitude"] = "AIR_GROUND";
            rules["channels"]!["engineN1"] = "NOT_A_PARAMETER";
            rules["coverage"]!["requiredChannels"]![0] = "N1_PCT[3]";
            rules["affects"]!.AsObject().Add("NOT_A_SPAN", new JsonArray("PHASE_ALTITUDE_RATE"));
            rules.Remove("clock");
            rules["grade"]!["masksAllowedForA"]![0]!["cause"] = "NOT_A_CAUSE";
            rules["stuckSensor"]!["channels"]![2]!["minN1ChangePct"] = 0;
        });

        var issues = QualityRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues;

        Assert.Contains(issues, issue => issue.Path == path && issue.Code == code);
    }

    [Fact]
    public void An_unknown_field_is_refused()
    {
        var content = SeedDocuments.Changed(Seed, rules => rules["formula"] = "a > b");

        var issues = QualityRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues;

        Assert.Contains(issues, issue => issue.Code == ContentJson.ShapeInvalid);
    }
}
