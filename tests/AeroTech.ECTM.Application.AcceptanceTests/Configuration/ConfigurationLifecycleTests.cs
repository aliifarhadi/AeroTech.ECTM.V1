using System.Net;
using System.Text.Json.Nodes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Configuration;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class ConfigurationLifecycleTests(MigratedDatabaseFixture fixture)
{
    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task A_kind_of_a_later_phase_cannot_be_authored_yet()
    {
        using var response = await _configuration.PostAsync("config/CORRECTION_MODEL/documents", new { content = new { rules = 1 } });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80105, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Publishing_without_an_approval_note_is_rejected()
    {
        var draft = await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { content = ConfigurationApi.SeedOf(ConfigurationApi.Catalog) });

        using var response = await _configuration.PublishAsync(ConfigurationApi.Catalog, FleetApi.IdOf(draft), "");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task An_invalid_draft_shows_its_issues_and_cannot_be_published()
    {
        var content = ConfigurationApi.SeedOf(ConfigurationApi.Catalog);
        var parameters = content["parameters"]!.AsArray();
        parameters.Add(parameters[0]!.DeepClone());
        var draft = await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { content });
        var draftId = FleetApi.IdOf(draft);

        using var validation = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/validate", new { });
        var result = await ApiResponses.DataOf(validation);
        using var published = await _configuration.PublishAsync(ConfigurationApi.Catalog, draftId, "should fail");

        Assert.False(result.GetProperty("valid").GetBoolean());
        Assert.Contains(result.GetProperty("issues").EnumerateArray(), issue => issue.GetProperty("code").GetString() == "CODE_DUPLICATE");
        Assert.Equal((HttpStatusCode)422, published.StatusCode);
        Assert.Equal(80108, await ApiResponses.FirstErrorCodeOf(published));
    }

    [Fact]
    public async Task A_catalog_that_would_break_the_active_profile_cannot_be_published()
    {
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        await _configuration.EnsureActiveAsync(ConfigurationApi.Profile);

        var content = ConfigurationApi.SeedOf(ConfigurationApi.Catalog);
        var parameters = content["parameters"]!.AsArray();
        var mach = parameters.Single(parameter => parameter!["code"]!.GetValue<string>() == "MACH");
        parameters.Remove(mach);
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { content }));

        using var validation = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/validate", new { });
        var issues = (await ApiResponses.DataOf(validation)).GetProperty("issues").EnumerateArray().ToList();

        Assert.Contains(issues, issue => issue.GetProperty("code").GetString() == "ACTIVE_PROFILE_INCOMPATIBLE"
                                         && issue.GetProperty("message").GetString()!.Contains("MACH"));
    }

    [Fact]
    public async Task A_draft_cannot_be_retired_and_a_published_version_can()
    {
        var activeCatalogId = await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { basedOnDocumentId = activeCatalogId }));

        using (var retireDraft = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/retire", new { note = "too early" }))
        {
            Assert.Equal(HttpStatusCode.Conflict, retireDraft.StatusCode);
            Assert.Equal(80107, await ApiResponses.FirstErrorCodeOf(retireDraft));
        }

        using (var published = await _configuration.PublishAsync(ConfigurationApi.Catalog, draftId, "temporary version"))
            Assert.Equal(HttpStatusCode.OK, published.StatusCode);

        using var retired = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/retire", new { note = "superseded" });
        var document = await ApiResponses.DataOf(retired);

        Assert.Equal("RETIRED", document.GetProperty("status").GetProperty("name").GetString());
        Assert.Equal("superseded", document.GetProperty("retirementNote").GetString());
        Assert.NotEqual(draftId, FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.Catalog)));
    }

    [Fact]
    public async Task A_draft_cannot_be_based_on_a_document_of_another_kind()
    {
        var profileDraft = await _configuration.CreateDraftAsync(ConfigurationApi.Profile, new { content = ConfigurationApi.SeedOf(ConfigurationApi.Profile) });

        using var response = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents", new { basedOnDocumentId = FleetApi.IdOf(profileDraft) });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80110, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Publishing_writes_an_audit_entry_with_the_status_change_only()
    {
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { content = ConfigurationApi.SeedOf(ConfigurationApi.Catalog) }));

        using (var published = await _configuration.PublishAsync(ConfigurationApi.Catalog, draftId, "audited publication"))
            Assert.Equal(HttpStatusCode.OK, published.StatusCode);

        using var audit = await _configuration.GetAsync($"audit?entityType=ConfigurationDocument&entityId={draftId}");
        var entries = (await ApiResponses.DataOf(audit)).GetProperty("results").EnumerateArray().ToList();
        var update = entries.Single(entry => entry.GetProperty("action").GetProperty("name").GetString() == "UPDATED");
        var after = update.GetProperty("after");

        Assert.Equal("PUBLISHED", after.GetProperty("Status").GetString());
        Assert.Equal("audited publication", after.GetProperty("ApprovalNote").GetString());
        Assert.False(after.TryGetProperty("Content", out _));
    }
}
