using System.Net;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fleet;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class EngineModelTests(MigratedDatabaseFixture fixture)
{
    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task An_engine_model_keeps_its_documents_and_can_be_edited()
    {
        var code = FleetApi.UniqueCode("MODEL");

        var created = await _fleet.CreateEngineModelAsync(code);
        var engineModelId = FleetApi.IdOf(created);

        Assert.Equal(code, created.GetProperty("code").GetString());
        Assert.Equal("TEST-ONLY model, no OEM source", created.GetProperty("documentReferences")[0].GetString());

        using var updated = await _fleet.PutAsync($"engine-models/{engineModelId}", new
        {
            manufacturer = FleetApi.Manufacturer,
            family = "TEST-FAMILY",
            documentReferences = new[] { "doc A", "doc B" }
        });
        var engineModel = await ApiResponses.DataOf(updated);

        Assert.Equal("TEST-FAMILY", engineModel.GetProperty("family").GetString());
        Assert.Equal(2, engineModel.GetProperty("documentReferences").GetArrayLength());
        Assert.Equal(code, engineModel.GetProperty("code").GetString());
    }

    [Fact]
    public async Task A_duplicate_code_is_refused()
    {
        var code = FleetApi.UniqueCode("MODEL");
        await _fleet.CreateEngineModelAsync(code);

        using var response = await _fleet.PostAsync("engine-models", new { code, manufacturer = "OTHER" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(80204, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_model_without_manufacturer_is_rejected()
    {
        using var response = await _fleet.PostAsync("engine-models", new { code = FleetApi.UniqueCode("MODEL"), manufacturer = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
