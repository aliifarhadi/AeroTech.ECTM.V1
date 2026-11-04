using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class ParserSetupWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string EgtOverN2 = """
        {"code":"EGT_OVER_N2","kind":"RATIO","role":"ENGINE_PERFORMANCE","unitOut":"CELSIUS_PER_PERCENT","status":"CANDIDATE",
         "description":"EGT per percent N2 (owner example)","numerator":"EGT_C[pos]","denominator":"N2_PCT[pos]","guardZero":true}
        """;

    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task J2_the_owner_activates_the_parser_and_adds_EGT_OVER_N2_as_a_new_catalog_version()
    {
        var activeCatalogId = await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        await _configuration.EnsureActiveAsync(ConfigurationApi.Profile);

        using (var readiness = await _configuration.GetAsync("meta/readiness"))
        {
            var data = await ApiResponses.DataOf(readiness);
            Assert.True(data.GetProperty("parserReady").GetBoolean());
            Assert.Empty(data.GetProperty("missingForParser").EnumerateArray());
        }

        var draft = await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { basedOnDocumentId = activeCatalogId });
        var draftId = FleetApi.IdOf(draft);
        Assert.Equal("DRAFT", draft.GetProperty("status").GetProperty("name").GetString());
        Assert.Equal(activeCatalogId, draft.GetProperty("basedOnDocumentId").GetString());

        var content = await _configuration.ContentOfAsync(ConfigurationApi.Catalog, draftId);
        var derivedCountBefore = content["derivedParameters"]!.AsArray().Count;
        content["derivedParameters"]!.AsArray().Add(JsonNode.Parse(EgtOverN2));

        using (var edited = await _configuration.PutAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}", new { content }))
            Assert.Equal(HttpStatusCode.OK, edited.StatusCode);

        using (var validation = await _configuration.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/validate", new { }))
            Assert.True((await ApiResponses.DataOf(validation)).GetProperty("valid").GetBoolean());

        using (var evaluation = await _configuration.PostAsync("config/typed-definitions/evaluate", new
        {
            catalogDocumentId = draftId,
            definition = JsonNode.Parse(EgtOverN2),
            position = 1,
            inputs = new Dictionary<string, object>
            {
                ["EGT_C[1]"] = new { value = 600, unit = "CELSIUS" },
                ["N2_PCT[1]"] = new { value = 96, unit = "PERCENT" }
            }
        }))
        {
            var result = await ApiResponses.DataOf(evaluation);
            Assert.Equal("AVAILABLE", result.GetProperty("status").GetString());
            Assert.Equal(6.25, result.GetProperty("value").GetDouble());
            Assert.Equal("EGT_C[1] / N2_PCT[1] = 600 / 96 = 6.25", result.GetProperty("formula").GetString());
        }

        using (var published = await _configuration.PublishAsync(ConfigurationApi.Catalog, draftId, "owner adds EGT_OVER_N2"))
        {
            var document = await ApiResponses.DataOf(published);
            Assert.Equal("PUBLISHED", document.GetProperty("status").GetProperty("name").GetString());
            Assert.Equal("owner adds EGT_OVER_N2", document.GetProperty("approvalNote").GetString());
            Assert.Equal(fixture.Clock.Now, document.GetProperty("publishedUtc").GetDateTimeOffset());
        }

        Assert.Equal(draftId, FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.Catalog)));

        using (var diff = await _configuration.GetAsync($"config/{ConfigurationApi.Catalog}/documents/{activeCatalogId}/diff/{draftId}"))
        {
            var change = Assert.Single((await ApiResponses.DataOf(diff)).GetProperty("changes").EnumerateArray());
            Assert.Equal("ADDED", change.GetProperty("change").GetString());
            Assert.Equal($"$.derivedParameters[{derivedCountBefore}]", change.GetProperty("path").GetString());
            Assert.Equal("EGT_OVER_N2", change.GetProperty("after").GetProperty("code").GetString());
        }

        using (var frozen = await _configuration.PutAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}", new { content }))
        {
            Assert.Equal(HttpStatusCode.Conflict, frozen.StatusCode);
            Assert.Equal(80106, await ApiResponses.FirstErrorCodeOf(frozen));
        }

        using (var readinessAfter = await _configuration.GetAsync("meta/readiness"))
            Assert.True((await ApiResponses.DataOf(readinessAfter)).GetProperty("parserReady").GetBoolean());
    }
}
