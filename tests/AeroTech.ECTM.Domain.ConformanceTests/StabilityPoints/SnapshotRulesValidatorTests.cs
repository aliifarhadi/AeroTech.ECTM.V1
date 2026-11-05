using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.StabilityPoints;

public sealed class SnapshotRulesValidatorTests
{
    [Fact]
    public void The_seed_is_valid_against_the_catalog_seed()
    {
        var validation = SnapshotRulesValidator.Validate(SnapshotParity.SeedContent, SeedDocuments.ValidatedCatalog);

        Assert.Empty(validation.Issues);
        Assert.Equal((CalibrationStatus.CANDIDATE, EngineeringSignOff.PENDING), (validation.Rules!.CalibrationStatus, validation.Rules.EngineeringSignOff));
        Assert.Equal(["STRICT", "OPERATOR_300S", "OPERATOR_60S"], validation.Rules.Cruise!.Selectors!.Select(selector => selector.Code));
        Assert.Contains("ENGINE_BLEED", validation.ParameterCodes);
    }

    [Theory]
    [InlineData("$.channels.engineN1", RuleDocumentCheck.ChannelKindInvalid)]
    [InlineData("$.engineParameters[3]", RuleDocumentCheck.ChannelUnknown)]
    [InlineData("$.configurationChannels[0]", RuleDocumentCheck.ChannelKindInvalid)]
    [InlineData("$.masks.coherence[0]", RuleDocumentCheck.ReferenceUnknown)]
    [InlineData("$.cruise.selectors[0].reportSec", RuleDocumentCheck.ValueInvalid)]
    [InlineData("$.cruise.selectors[2].code", RuleDocumentCheck.CodeDuplicate)]
    [InlineData("$.takeoff.partialStartN1Pct", RuleDocumentCheck.ValueInvalid)]
    [InlineData("$.decisions[0].appliesTo[1]", RuleDocumentCheck.ReferenceUnknown)]
    [InlineData("$.title", RuleDocumentCheck.FieldRequired)]
    public void A_broken_document_names_the_path_of_the_problem(string path, string code)
    {
        var content = SeedDocuments.Changed(SnapshotParity.SeedContent, rules =>
        {
            rules["channels"]!["engineN1"] = "MACH";
            rules["engineParameters"]![3] = "NOT_A_PARAMETER";
            rules["configurationChannels"]![0] = "CAS_KT";
            rules["masks"]!["coherence"]![0] = "NOT_A_MASK";
            rules["cruise"]!["selectors"]![0]!["reportSec"] = 400;
            rules["cruise"]!["selectors"]![2]!["code"] = "STRICT";
            rules["takeoff"]!["partialStartN1Pct"] = 140;
            rules["decisions"]![0]!["appliesTo"]![1] = "cruise/NOT_A_SELECTOR";
            rules.Remove("title");
        });

        var issues = SnapshotRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues;

        Assert.Contains(issues, issue => issue.Path == path && issue.Code == code);
    }

    [Fact]
    public void An_unknown_field_is_refused()
    {
        var content = SeedDocuments.Changed(SnapshotParity.SeedContent, rules => rules["defaultSelector"] = "STRICT");

        var issue = Assert.Single(SnapshotRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
    }
}
