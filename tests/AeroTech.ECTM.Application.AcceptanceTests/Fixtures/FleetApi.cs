using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class FleetApi(HttpClient client)
{
    public const string Manufacturer = "TEST-MANUFACTURER";

    public static string UniqueRegistration() => $"T-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    public static string UniqueCode(string prefix) => $"{prefix}-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}";

    public Task<HttpResponseMessage> PostAsync(string route, object body) => client.PostAsJsonAsync($"Backoffice/v1/{route}", body);

    public Task<HttpResponseMessage> PutAsync(string route, object body) => client.PutAsJsonAsync($"Backoffice/v1/{route}", body);

    public Task<HttpResponseMessage> GetAsync(string route) => client.GetAsync($"Backoffice/v1/{route}");

    public Task<JsonElement> CreateAircraftAsync(string registration)
        => CreatedAsync("aircraft", new { registration, typeCode = "B737", operatorCode = "TEST" });

    public Task<JsonElement> CreateEngineModelAsync(string code)
        => CreatedAsync("engine-models", new { code, manufacturer = Manufacturer, documentReferences = new[] { "TEST-ONLY model, no OEM source" } });

    public Task<JsonElement> CreateEngineAsync(string serialNumber, string engineModelId)
        => CreatedAsync("engines", new { serialNumber, engineModelId, status = "ACTIVE" });

    public Task<HttpResponseMessage> InstallAsync(string engineId, string aircraftId, int position, DateTimeOffset installedUtc)
        => PostAsync("installations", new { engineId, aircraftId, position, installedUtc, reason = "acceptance test installation" });

    public async Task<JsonElement> CreatedAsync(string route, object body)
    {
        using var response = await PostAsync(route, body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    public static string IdOf(JsonElement element) => element.GetProperty("id").GetString()!;
}
