using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Configuration;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class ConfigurationKindsTests(MigratedDatabaseFixture fixture)
{
    private static readonly string[] KindsInOrder =
    [
        "DATA_PROFILE",
        "PARAMETER_CATALOG",
        "QUALITY_RULES",
        "PHASE_RULESET",
        "SNAPSHOT_RULES",
        "BASELINE_RULES",
        "CORRECTION_MODEL",
        "ANALYTICS_RULES",
        "REPORT_DEFINITION"
    ];

    private static readonly Dictionary<string, string> AuthorableNow = new()
    {
        ["DATA_PROFILE"] = "P02",
        ["PARAMETER_CATALOG"] = "P02",
        ["QUALITY_RULES"] = "P04",
        ["PHASE_RULESET"] = "P05"
    };

    [Fact]
    public async Task Every_kind_is_listed_and_kinds_of_later_phases_have_no_active_version()
    {
        var kinds = await GetKindsAsync();

        var names = kinds.EnumerateArray().Select(kind => kind.GetProperty("kind").GetProperty("name").GetString()).ToList();
        Assert.Equal(KindsInOrder, names);

        foreach (var kind in kinds.EnumerateArray())
        {
            var name = kind.GetProperty("kind").GetProperty("name").GetString()!;
            var authorable = AuthorableNow.ContainsKey(name);

            Assert.Equal(authorable, kind.GetProperty("authorable").GetBoolean());

            if (authorable)
                continue;

            Assert.Equal(JsonValueKind.Null, kind.GetProperty("activeVersion").ValueKind);
            Assert.Equal("NO_ACTIVE_VERSION", kind.GetProperty("activeVersionReason").GetString());
            Assert.Equal(0, kind.GetProperty("publishedCount").GetInt32());
        }
    }

    [Fact]
    public async Task The_seed_drafts_of_the_authorable_kinds_exist_on_a_new_system()
    {
        var kinds = await GetKindsAsync();

        foreach (var (name, deliveryPhase) in AuthorableNow)
        {
            var kind = KindNamed(kinds, name);
            var documents = kind.GetProperty("draftCount").GetInt32() + kind.GetProperty("publishedCount").GetInt32();

            Assert.True(documents >= 1, $"{name} has no seed document.");
            Assert.Equal(deliveryPhase, kind.GetProperty("deliveryPhase").GetString());
        }
    }

    [Fact]
    public async Task A_new_draft_is_counted_but_does_not_become_active()
    {
        var draftsBefore = DraftCountOf(await GetKindsAsync(), "REPORT_DEFINITION");

        await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.REPORT_DEFINITION, FixedActorResolver.Alice);

        var kindsAfter = await GetKindsAsync();
        var reportDefinition = KindNamed(kindsAfter, "REPORT_DEFINITION");

        Assert.Equal(draftsBefore + 1, reportDefinition.GetProperty("draftCount").GetInt32());
        Assert.Equal(JsonValueKind.Null, reportDefinition.GetProperty("activeVersion").ValueKind);
        Assert.Equal("NO_ACTIVE_VERSION", reportDefinition.GetProperty("activeVersionReason").GetString());
    }

    private async Task<JsonElement> GetKindsAsync()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/kinds");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    private static int DraftCountOf(JsonElement kinds, string name)
        => KindNamed(kinds, name).GetProperty("draftCount").GetInt32();

    private static JsonElement KindNamed(JsonElement kinds, string name)
        => kinds.EnumerateArray().Single(kind => kind.GetProperty("kind").GetProperty("name").GetString() == name);
}
