using System.Net;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fleet;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class AircraftTests(MigratedDatabaseFixture fixture)
{
    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task An_aircraft_is_created_read_listed_and_edited()
    {
        var registration = FleetApi.UniqueRegistration();

        var created = await _fleet.CreateAircraftAsync(registration.ToLowerInvariant());
        var aircraftId = FleetApi.IdOf(created);

        Assert.Equal(registration, created.GetProperty("registration").GetString());
        Assert.Equal(2, created.GetProperty("engineCount").GetInt32());
        Assert.Equal("ACTIVE", created.GetProperty("status").GetProperty("name").GetString());
        Assert.Empty(created.GetProperty("currentInstallations").EnumerateArray());

        using var listed = await _fleet.GetAsync($"aircraft?search={registration}");
        var page = await ApiResponses.DataOf(listed);
        Assert.Equal(aircraftId, FleetApi.IdOf(page.GetProperty("results")[0]));

        using var updated = await _fleet.PutAsync($"aircraft/{aircraftId}", new { typeCode = "B737-500", operatorCode = "TEST", status = "STORED" });
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);

        using var read = await _fleet.GetAsync($"aircraft/{aircraftId}");
        var aircraft = await ApiResponses.DataOf(read);
        Assert.Equal("B737-500", aircraft.GetProperty("typeCode").GetString());
        Assert.Equal("STORED", aircraft.GetProperty("status").GetProperty("name").GetString());
    }

    [Fact]
    public async Task A_duplicate_registration_is_refused()
    {
        var registration = FleetApi.UniqueRegistration();
        await _fleet.CreateAircraftAsync(registration);

        using var response = await _fleet.PostAsync("aircraft", new { registration });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(80201, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_blank_registration_is_rejected()
    {
        using var response = await _fleet.PostAsync("aircraft", new { registration = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task An_unknown_aircraft_is_not_found()
    {
        using var response = await _fleet.GetAsync("aircraft/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(80200, await ApiResponses.FirstErrorCodeOf(response));
    }
}
