using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class ConfigurationApi(HttpClient client)
{
    public const string Catalog = "PARAMETER_CATALOG";
    public const string Profile = "DATA_PROFILE";
    public const string QualityRules = "QUALITY_RULES";

    public static JsonNode SeedOf(string kind) => JsonNode.Parse(ConfigurationSeeds.ContentOf(Enum.Parse<ConfigurationKind>(kind)))!;

    public async Task<string> EnsureActiveAsync(string kind)
    {
        var active = await ActiveVersionOfAsync(kind);

        if (active.ValueKind == JsonValueKind.Object)
            return FleetApi.IdOf(active);

        var draft = await CreateDraftAsync(kind, new { content = SeedOf(kind) });
        var draftId = FleetApi.IdOf(draft);

        using var response = await PublishAsync(kind, draftId, "acceptance test activation of the seed");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return draftId;
    }

    public async Task<JsonElement> ActiveVersionOfAsync(string kind)
    {
        using var response = await client.GetAsync("Backoffice/v1/config/kinds");
        var kinds = await ApiResponses.DataOf(response);

        return kinds.EnumerateArray()
            .Single(item => item.GetProperty("kind").GetProperty("name").GetString() == kind)
            .GetProperty("activeVersion");
    }

    public async Task<JsonElement> CreateDraftAsync(string kind, object body)
    {
        using var response = await PostAsync($"config/{kind}/documents", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    public async Task<JsonObject> ContentOfAsync(string kind, string documentId)
    {
        using var response = await GetAsync($"config/{kind}/documents/{documentId}");
        var document = await ApiResponses.DataOf(response);

        return JsonNode.Parse(document.GetProperty("content").GetRawText())!.AsObject();
    }

    public Task<HttpResponseMessage> PublishAsync(string kind, string documentId, string approvalNote)
        => PostAsync($"config/{kind}/documents/{documentId}/publish", new { approvalNote });

    public Task<HttpResponseMessage> PostAsync(string route, object body) => client.PostAsJsonAsync($"Backoffice/v1/{route}", body);

    public Task<HttpResponseMessage> PutAsync(string route, object body) => client.PutAsJsonAsync($"Backoffice/v1/{route}", body);

    public Task<HttpResponseMessage> GetAsync(string route) => client.GetAsync($"Backoffice/v1/{route}");

    public Task<HttpResponseMessage> PostFileAsync(string route, string fileName, byte[] content)
    {
        var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        form.Add(file, "file", fileName);

        return client.PostAsync($"Backoffice/v1/{route}", form);
    }
}
