using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public sealed class PhaseReferenceTests
{
    [Fact]
    public void The_seed_matches_the_pack_reference()
    {
        var reference = PhaseReference.Compare(PhaseParity.SeedRules);

        Assert.Equal(("phase-v1.0.3", ReferenceMatch.MATCHES_REFERENCE), (reference.Algorithm, reference.Match));
        Assert.Empty(reference.Changes);
    }

    [Fact]
    public void Display_text_and_status_do_not_change_the_match()
    {
        var edited = SeedDocuments.Changed(PhaseParity.SeedContent, rules =>
        {
            rules["title"] = "another title";
            rules["description"] = "another description";
            rules["source"] = "another source";
            rules["calibrationStatus"] = "CALIBRATED";
            rules["engineeringSignOff"] = "APPROVED";
            rules["events"]![0]!["note"] = "another note";
            rules["decisions"]![0]!["choice"] = "another choice";
        });

        var reference = PhaseReference.Compare(PhaseParity.Validated(edited));

        Assert.Equal(ReferenceMatch.MATCHES_REFERENCE, reference.Match);
    }

    [Fact]
    public void A_changed_rule_is_reported_with_its_path_and_values()
    {
        var reference = PhaseReference.Compare(PhaseParity.Validated(PhaseExplanationTests.CruiseMinimumDraft(240)));
        var change = Assert.Single(reference.Changes);

        Assert.Equal(ReferenceMatch.MODIFIED_FROM_REFERENCE, reference.Match);
        Assert.EndsWith(".minSec", change.Path);
        Assert.Equal(("180", "240"), (change.Before, change.After));
    }

    [Fact]
    public void A_changed_touchdown_pick_is_reported()
    {
        var edited = SeedDocuments.Changed(PhaseParity.SeedContent, rules => rules["anchors"]![1]!["pick"] = JsonValue.Create("LAST"));

        var change = Assert.Single(PhaseReference.Compare(PhaseParity.Validated(edited)).Changes);

        Assert.EndsWith(".pick", change.Path);
    }
}
