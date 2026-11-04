using System.Net.Http.Json;
using System.Text.Json;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public static class ApiResponses
{
    public static async Task<JsonElement> DataOf(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        return body.GetProperty("data");
    }

    public static async Task<int> FirstErrorCodeOf(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        return body.GetProperty("errors")[0].GetProperty("code").GetInt32();
    }
}
