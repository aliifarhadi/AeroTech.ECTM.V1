using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class FleetSetupWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string Registration = "EP-DOA";
    private const string EngineOneSerial = "Eng1S1";
    private const string EngineTwoSerial = "Eng2S2";
    private const string SpareSerial = "TEST-ESN-SPARE-NOT-REAL";
    private const string UnverifiedModel = "TEST-MODEL-UNVERIFIED";

    private static readonly DateTimeOffset InstalledOnSeptember24 = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset WashOnOctober5 = new(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);

    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task J1_the_owner_describes_EP_DOA_with_both_engines_and_their_history()
    {
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(Registration));
        var engineModelId = FleetApi.IdOf(await _fleet.CreatedAsync("engine-models", new
        {
            code = UnverifiedModel,
            manufacturer = "UNVERIFIED",
            documentReferences = new[] { "model not supplied by the owner yet (I-01); test label, not an OEM identity" }
        }));

        var engineOne = FleetApi.IdOf(await _fleet.CreateEngineAsync(EngineOneSerial, engineModelId));
        var engineTwo = FleetApi.IdOf(await _fleet.CreateEngineAsync(EngineTwoSerial, engineModelId));
        var spare = FleetApi.IdOf(await _fleet.CreateEngineAsync(SpareSerial, engineModelId));

        using (var first = await _fleet.InstallAsync(engineOne, aircraftId, 1, InstalledOnSeptember24))
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        using (var second = await _fleet.InstallAsync(engineTwo, aircraftId, 2, InstalledOnSeptember24))
            Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        using (var overlap = await _fleet.InstallAsync(spare, aircraftId, 1, WashOnOctober5))
        {
            Assert.Equal(HttpStatusCode.Conflict, overlap.StatusCode);
            var message = (await overlap.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("errors")[0].GetProperty("title").GetString()!;
            Assert.Contains($"engine {EngineOneSerial} on aircraft {Registration} position 1 from 2026-09-24T00:00:00Z to open", message);
        }

        await _fleet.CreatedAsync("maintenance-events", new
        {
            engineId = engineOne,
            eventType = "WASH",
            eventUtc = WashOnOctober5,
            dateReliability = "DAY_ONLY"
        });

        using var aircraftResponse = await _fleet.GetAsync($"aircraft/{aircraftId}");
        var aircraft = await ApiResponses.DataOf(aircraftResponse);
        var positions = aircraft.GetProperty("currentInstallations").EnumerateArray()
            .Select(installation => (installation.GetProperty("position").GetInt32(), installation.GetProperty("engineSerialNumber").GetString()))
            .ToList();

        Assert.Equal([(1, EngineOneSerial), (2, EngineTwoSerial)], positions);

        using var maintenance = await _fleet.GetAsync($"maintenance-events?engineId={engineOne}");
        var wash = Assert.Single((await ApiResponses.DataOf(maintenance)).GetProperty("results").EnumerateArray());
        Assert.Equal("WASH", wash.GetProperty("eventType").GetProperty("name").GetString());
    }
}
