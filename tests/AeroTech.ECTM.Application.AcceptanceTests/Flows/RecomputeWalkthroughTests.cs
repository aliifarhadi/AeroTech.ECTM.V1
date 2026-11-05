using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(RecomputeDatabaseCollection.Name)]
public sealed class RecomputeWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string Flight9072 = "2026-09-24_EP-DOA_9072.csv";
    private const int SamplesInCruisesShorterThan240Sec = 213 + 184;

    private readonly ImportApi _imports = new(fixture);
    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task Publish_and_recompute_creates_a_new_revision_and_keeps_the_old_one_for_audit()
    {
        await _imports.EnsureFleetAsync();
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        await _configuration.EnsureActiveAsync(ConfigurationApi.Profile);
        var qualityRulesId = await _configuration.EnsureActiveAsync(ConfigurationApi.QualityRules);

        var preview = await _imports.PreviewedAsync((Flight9072, ImportApi.RealBytes(Flight9072)));
        var batch = await _imports.CommittedAsync(FleetApi.IdOf(preview), "recompute-commit");
        var flightId = ImportApi.ItemNamed(batch, Flight9072).GetProperty("flightId").GetString()!;

        using (var noPhases = await _imports.GetAsync($"flights/{flightId}/phases"))
        {
            Assert.Equal(HttpStatusCode.NotFound, noPhases.StatusCode);
            Assert.Equal(80319, await ApiResponses.FirstErrorCodeOf(noPhases));
        }

        var firstQuality = await _imports.GetDataAsync($"flights/{flightId}/quality");
        var first = Assert.Single((await RevisionsAsync(flightId)).EnumerateArray());
        Assert.Equal(JsonValueKind.Null, first.GetProperty("phaseRuleSetDocumentId").ValueKind);

        var seedVersionId = await _configuration.EnsureActiveAsync(ConfigurationApi.PhaseRuleSet);
        var second = await RecomputeAsync(flightId, new { });

        Assert.Equal((2, "PENDING", "RECOMPUTE"), (second.GetProperty("number").GetInt32(), ImportApi.NameOf(second, "status"), ImportApi.NameOf(second, "trigger")));

        using (var whilePending = await _imports.PostAsync($"flights/{flightId}/recompute", new { }))
        {
            Assert.Equal(HttpStatusCode.Conflict, whilePending.StatusCode);
            Assert.Equal(80322, await ApiResponses.FirstErrorCodeOf(whilePending));
        }

        await _imports.RunJobsAsync();

        var revisions = (await RevisionsAsync(flightId)).EnumerateArray().ToList();
        var secondPhases = await _imports.GetDataAsync($"flights/{flightId}/phases");
        var secondQuality = await _imports.GetDataAsync($"flights/{flightId}/quality");

        Assert.Equal([(2, "CURRENT"), (1, "SUPERSEDED")], revisions.Select(revision => (revision.GetProperty("number").GetInt32(), ImportApi.NameOf(revision, "status"))));
        Assert.Equal(seedVersionId, revisions[0].GetProperty("phaseRuleSetDocumentId").GetString());
        Assert.Equal(2, secondPhases.GetProperty("revisionNumber").GetInt32());
        Assert.NotEqual(FleetApi.IdOf(firstQuality), FleetApi.IdOf(secondQuality));
        Assert.Equal(ImportApi.NameOf(firstQuality, "grade"), ImportApi.NameOf(secondQuality, "grade"));

        var longerCruiseId = await PublishLongerCruiseMinimumAsync(seedVersionId);
        var dryRun = await _imports.PostAsync($"flights/{flightId}/recompute", new { dryRun = true });
        var preview240 = (await ApiResponses.DataOf(dryRun)).GetProperty("preview");

        Assert.Equal(SamplesInCruisesShorterThan240Sec, preview240.GetProperty("changedSamples").GetInt32());
        Assert.Equal(longerCruiseId, preview240.GetProperty("proposedPhaseRuleSetDocumentId").GetString());

        await RecomputeAsync(flightId, new { });
        await _imports.RunJobsAsync();

        var thirdPhases = await _imports.GetDataAsync($"flights/{flightId}/phases");
        Assert.Equal(3, thirdPhases.GetProperty("revisionNumber").GetInt32());
        Assert.Equal("MODIFIED_FROM_REFERENCE", ImportApi.NameOf(thirdPhases.GetProperty("reference"), "match"));
        Assert.Single(thirdPhases.GetProperty("reference").GetProperty("changes").EnumerateArray());
        Assert.Equal(CruiseSamplesOf(secondPhases) - SamplesInCruisesShorterThan240Sec, CruiseSamplesOf(thirdPhases));

        await RecomputeAsync(flightId, new { phaseRuleSetDocumentId = seedVersionId });
        await _imports.RunJobsAsync();

        var fourthPhases = await _imports.GetDataAsync($"flights/{flightId}/phases");
        Assert.Equal(4, fourthPhases.GetProperty("revisionNumber").GetInt32());
        Assert.Equal(secondPhases.GetProperty("segments").GetRawText(), fourthPhases.GetProperty("segments").GetRawText());

        using (var wrongKind = await _imports.PostAsync($"flights/{flightId}/recompute", new { phaseRuleSetDocumentId = qualityRulesId }))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, wrongKind.StatusCode);
            Assert.Equal(80323, await ApiResponses.FirstErrorCodeOf(wrongKind));
        }

        var statuses = (await RevisionsAsync(flightId)).EnumerateArray().Select(revision => ImportApi.NameOf(revision, "status"));
        Assert.Equal(["CURRENT", "SUPERSEDED", "SUPERSEDED", "SUPERSEDED"], statuses);
    }

    private async Task<string> PublishLongerCruiseMinimumAsync(string basedOnId)
    {
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.PhaseRuleSet, new { basedOnDocumentId = basedOnId }));
        var content = await _configuration.ContentOfAsync(ConfigurationApi.PhaseRuleSet, draftId);
        var cruise = content["conditions"]!.AsArray().Single(condition => condition!["code"]!.GetValue<string>() == "CRUISE")!;

        cruise["minSec"] = 240;

        using (var edited = await _configuration.PutAsync($"config/{ConfigurationApi.PhaseRuleSet}/documents/{draftId}", new { content }))
            Assert.Equal(HttpStatusCode.OK, edited.StatusCode);

        using (var published = await _configuration.PublishAsync(ConfigurationApi.PhaseRuleSet, draftId, "acceptance test: cruise minimum 240 s"))
            Assert.Equal(HttpStatusCode.OK, published.StatusCode);

        return draftId;
    }

    private async Task<JsonElement> RecomputeAsync(string flightId, object body)
    {
        using var response = await _imports.PostAsync($"flights/{flightId}/recompute", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var recompute = await ApiResponses.DataOf(response);

        Assert.False(recompute.GetProperty("dryRun").GetBoolean());
        Assert.Equal(JsonValueKind.String, recompute.GetProperty("processingJobId").ValueKind);

        return recompute.GetProperty("revision");
    }

    private Task<JsonElement> RevisionsAsync(string flightId) => _imports.GetDataAsync($"flights/{flightId}/revisions");

    private static int CruiseSamplesOf(JsonElement phases)
        => phases.GetProperty("coverage").EnumerateArray()
            .Where(entry => ImportApi.NameOf(entry, "phase") == "CRUISE")
            .Sum(entry => entry.GetProperty("samples").GetInt32());
}
