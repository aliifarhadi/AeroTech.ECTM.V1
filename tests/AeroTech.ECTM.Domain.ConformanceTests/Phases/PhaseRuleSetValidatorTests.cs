using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public sealed class PhaseRuleSetValidatorTests
{
    [Fact]
    public void The_seed_is_valid_against_the_catalog_seed()
    {
        var validation = PhaseRuleSetValidator.Validate(PhaseParity.SeedContent, SeedDocuments.ValidatedCatalog);

        Assert.Empty(validation.Issues);
        Assert.Equal((CalibrationStatus.CANDIDATE, EngineeringSignOff.PENDING), (validation.Rules!.CalibrationStatus, validation.Rules.EngineeringSignOff));
        Assert.False(string.IsNullOrWhiteSpace(validation.Rules.Title));
        Assert.False(string.IsNullOrWhiteSpace(validation.Rules.Description));
    }

    [Fact]
    public void The_seed_reads_into_the_closed_operator_library()
    {
        var rules = PhaseParity.SeedRules;

        Assert.IsType<CentralDifferenceRate>(rules.DerivedChannels![0]);
        Assert.IsType<MedianSmooth>(rules.DerivedChannels[1]);
        Assert.IsType<FirstUpwardCrossing>(rules.Anchors![0]);
        Assert.Equal(TransitionPick.FIRST, Assert.IsType<SustainedTransition>(rules.Anchors[1]).Pick);
        Assert.Equal(9, rules.LabelRules!.Count);
        Assert.Equal(13, rules.Decisions!.Count);
    }

    [Theory]
    [InlineData("$.conditions[0].predicate.of[1].channel", PhaseRuleSetValidator.ChannelUnknown)]
    [InlineData("$.conditions[0].predicate.of[0].channel", PhaseRuleSetValidator.ChannelKindInvalid)]
    [InlineData("$.conditions[2].predicate.of[0].condition", PhaseRuleSetValidator.ReferenceUnknown)]
    [InlineData("$.anchors[0].at.condition", PhaseRuleSetValidator.ReferenceUnknown)]
    [InlineData("$.labelRules[4].when.mask", PhaseRuleSetValidator.ReferenceUnknown)]
    [InlineData("$.labelRules[4].unknownReason", PhaseRuleSetValidator.FieldRequired)]
    [InlineData("$.segments.evidenceCodes.LEVEL", PhaseRuleSetValidator.FieldRequired)]
    [InlineData("$.decisions[1].appliesTo[0]", PhaseRuleSetValidator.ReferenceUnknown)]
    [InlineData("$.events[0].activeValue", PhaseRuleSetValidator.ValueInvalid)]
    [InlineData("$.anchors[1].from", PhaseRuleSetValidator.ValueInvalid)]
    [InlineData("$.derivedChannels[1].minValid", PhaseRuleSetValidator.ValueInvalid)]
    [InlineData("$.title", PhaseRuleSetValidator.FieldRequired)]
    [InlineData("$.conditions[3].code", PhaseRuleSetValidator.CodeDuplicate)]
    public void A_broken_document_names_the_path_of_the_problem(string path, string code)
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules =>
        {
            var conditions = rules["conditions"]!.AsArray();

            conditions[0]!["predicate"]!["of"]![1]!["channel"] = "NOT_A_PARAMETER";
            conditions[0]!["predicate"]!["of"]![0]!["channel"] = "CAS_KT";
            conditions[2]!["predicate"]!["of"]![0]!["condition"] = "CRUISE";
            conditions[3]!["code"] = "CLIMB";
            rules["anchors"]![0]!["at"] = new JsonObject { ["operator"] = "IN_CONDITION", ["condition"] = "GROUND" };
            rules["anchors"]![1]!["from"] = "AIRBORNE";
            rules["labelRules"]![4]!["when"]!["mask"] = "NOT_A_MASK";
            rules["labelRules"]![4]!.AsObject().Remove("unknownReason");
            rules["segments"]!["evidenceCodes"]!.AsObject().Remove("LEVEL");
            rules["decisions"]![1]!["appliesTo"]![0] = "anchors/NOT_AN_ANCHOR";
            rules["events"]![0]!["activeValue"] = "Open";
            rules["derivedChannels"]![1]!["minValid"] = 9;
            rules.Remove("title");
        });

        var issues = PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues;

        Assert.Contains(issues, issue => issue.Path == path && issue.Code == code);
    }

    [Fact]
    public void An_operator_outside_the_closed_library_is_refused_with_its_path()
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["anchors"]![1]!["operator"] = "LAST_TRANSITION_SUSTAINED");

        var issue = Assert.Single(PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
        Assert.StartsWith("$.anchors[1]", issue.Path);
    }

    [Fact]
    public void An_english_instruction_is_refused_and_never_evaluated()
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["anchors"]![1]!["find"] = "last transition of AIR_GROUND from AIR to GND");

        var issue = Assert.Single(PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
    }

    [Fact]
    public void A_node_without_an_operator_is_refused()
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["conditions"]![0]!.AsObject().Remove("operator"));

        var issue = Assert.Single(PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
    }

    [Fact]
    public void A_comparison_written_as_a_number_is_refused()
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["anchors"]![0]!["at"]!["op"] = 3);

        var issue = Assert.Single(PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
    }

    [Fact]
    public void A_threshold_written_as_text_is_refused()
    {
        var content = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["anchors"]![0]!["at"]!["value"] = "100 kt");

        var issue = Assert.Single(PhaseRuleSetValidator.Validate(content, SeedDocuments.ValidatedCatalog).Issues);

        Assert.Equal(ContentJson.ShapeInvalid, issue.Code);
    }
}
