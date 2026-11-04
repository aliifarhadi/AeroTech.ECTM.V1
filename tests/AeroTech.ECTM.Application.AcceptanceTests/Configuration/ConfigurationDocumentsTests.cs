using System.Net;
using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Configuration;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class ConfigurationDocumentsTests(MigratedDatabaseFixture fixture)
{
    private const int KindNotFound = 80100;
    private const int DocumentNotFound = 80101;

    [Fact]
    public async Task A_seeded_draft_appears_in_the_list_of_its_kind()
    {
        var draft = await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.DATA_PROFILE, FixedActorResolver.Alice);

        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/DATA_PROFILE/documents?pageSize=100");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ApiResponses.DataOf(response);
        var listed = page.GetProperty("results").EnumerateArray().Single(document => document.GetProperty("id").GetString() == draft.Id.ToString());

        Assert.Equal("DATA_PROFILE", listed.GetProperty("kind").GetProperty("name").GetString());
        Assert.Equal(draft.VersionNumber, listed.GetProperty("versionNumber").GetInt32());
        Assert.Equal("DRAFT", listed.GetProperty("status").GetProperty("name").GetString());
        Assert.Equal(draft.ContentSha256, listed.GetProperty("contentSha256").GetString());
        Assert.False(listed.TryGetProperty("content", out _));
    }

    [Fact]
    public async Task The_detail_of_a_draft_returns_its_content_as_json()
    {
        var draft = await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.DATA_PROFILE, FixedActorResolver.Alice);

        using var response = await fixture.Client.GetAsync($"Backoffice/v1/config/DATA_PROFILE/documents/{draft.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await ApiResponses.DataOf(response);

        Assert.Equal(draft.Id.ToString(), document.GetProperty("id").GetString());
        Assert.Equal("acceptance test draft", document.GetProperty("content").GetProperty("purpose").GetString());
        Assert.Equal(draft.ContentSha256, document.GetProperty("contentSha256").GetString());
    }

    [Fact]
    public async Task An_unknown_kind_is_not_found()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/NOT_A_KIND/documents");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(KindNotFound, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Theory]
    [InlineData("data_profile")]
    [InlineData("1")]
    [InlineData("DATA_PROFILE,PARAMETER_CATALOG")]
    public async Task Only_the_exact_kind_code_is_accepted(string kind)
    {
        using var response = await fixture.Client.GetAsync($"Backoffice/v1/config/{kind}/documents");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(KindNotFound, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_document_requested_under_another_kind_is_not_found()
    {
        var draft = await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.DATA_PROFILE, FixedActorResolver.Alice);

        using var response = await fixture.Client.GetAsync($"Backoffice/v1/config/PARAMETER_CATALOG/documents/{draft.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(DocumentNotFound, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task An_unknown_document_is_not_found()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/DATA_PROFILE/documents/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(DocumentNotFound, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Theory]
    [InlineData("pageSize=0")]
    [InlineData("pageSize=101")]
    [InlineData("pageNumber=0")]
    public async Task A_page_outside_the_limits_is_rejected(string paging)
    {
        using var response = await fixture.Client.GetAsync($"Backoffice/v1/config/DATA_PROFILE/documents?{paging}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
