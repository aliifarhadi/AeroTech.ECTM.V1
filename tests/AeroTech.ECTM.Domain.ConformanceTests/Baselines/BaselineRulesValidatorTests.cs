using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Baselines;

public sealed class BaselineRulesValidatorTests
{
    private static readonly string SeedContent = ConfigurationSeeds.ContentOf(ConfigurationKind.BASELINE_RULES);

    [Fact]
    public void The_seed_is_valid_against_the_catalog_seed()
    {
        var validation = BaselineRulesValidator.Validate(SeedContent, SeedDocuments.ValidatedCatalog);

        Assert.Empty(validation.Issues);
        Assert.Equal((CalibrationStatus.CANDIDATE, EngineeringSignOff.PENDING), (validation.Rules!.CalibrationStatus, validation.Rules.EngineeringSignOff));
        Assert.Equal(["PRESSURE_ALT_FT", "MACH", "TAT_C", "N1_PCT"], validation.Rules.Conditions!.Select(condition => condition.Parameter));
    }

    [Theory]
    [InlineData("$.conditions[0].tolerance", RuleDocumentCheck.ValueInvalid)]
    [InlineData("$.conditions[3].parameter", RuleDocumentCheck.ChannelKindInvalid)]
    [InlineData("$.conditions[2].parameter", RuleDocumentCheck.CodeDuplicate)]
    [InlineData("$.decisions[1].appliesTo[0]", RuleDocumentCheck.ReferenceUnknown)]
    public void A_broken_document_names_the_path_of_the_problem(string path, string code)
    {
        var content = SeedDocuments.Changed(SeedContent, rules =>
        {
            rules["conditions"]![0]!["tolerance"] = 0;
            rules["conditions"]![3]!["parameter"] = "MACH";
            rules["conditions"]![2]!["parameter"] = "MACH";
            rules["decisions"]![1]!["appliesTo"]![0] = "conditions/NOT_A_CONDITION";
        });

        var issues = BaselineRulesValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues;

        Assert.Contains(issues, issue => issue.Path == path && issue.Code == code);
    }
}
