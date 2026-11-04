using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fleet;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class InstallationTests(MigratedDatabaseFixture fixture)
{
    private static readonly DateTimeOffset September24 = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset October10 = new(2026, 10, 10, 0, 0, 0, TimeSpan.Zero);

    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task An_engine_swap_in_a_dated_slot_keeps_both_periods_in_history()
    {
        var fleet = await NewFleetAsync();

        var first = await InstalledAsync(fleet.EngineOne, fleet.AircraftId, 1, September24);
        var closed = await CloseAsync(FleetApi.IdOf(first), October10);
        var second = await InstalledAsync(fleet.EngineTwo, fleet.AircraftId, 1, October10);

        Assert.Equal(October10, closed.GetProperty("removedUtc").GetDateTimeOffset());
        Assert.Equal(fleet.RegistrationOfAircraft, second.GetProperty("aircraftRegistration").GetString());

        using var history = await _fleet.GetAsync($"installations?aircraftId={fleet.AircraftId}&position=1");
        var installations = (await ApiResponses.DataOf(history)).GetProperty("results").EnumerateArray().ToList();
        Assert.Equal(2, installations.Count);
        Assert.Equal(FleetApi.IdOf(second), FleetApi.IdOf(installations[0]));

        var closedChanges = installations[1].GetProperty("changes").EnumerateArray().ToList();
        Assert.Equal(["INSTALLED", "REMOVED"], closedChanges.Select(change => change.GetProperty("changeType").GetProperty("name").GetString()));
        var installedVersion = long.Parse(closedChanges[0].GetProperty("registryVersion").GetString()!);
        var removedVersion = long.Parse(closedChanges[1].GetProperty("registryVersion").GetString()!);
        Assert.True(removedVersion > installedVersion);

        using var aircraftResponse = await _fleet.GetAsync($"aircraft/{fleet.AircraftId}");
        var current = (await ApiResponses.DataOf(aircraftResponse)).GetProperty("currentInstallations");
        Assert.Equal(fleet.SerialOfEngineTwo, Assert.Single(current.EnumerateArray()).GetProperty("engineSerialNumber").GetString());

        using var engineResponse = await _fleet.GetAsync($"engines/{fleet.EngineOne}");
        Assert.Equal(JsonValueKind.Null, (await ApiResponses.DataOf(engineResponse)).GetProperty("currentInstallation").ValueKind);
    }

    [Fact]
    public async Task An_overlap_on_the_same_position_is_refused_naming_the_conflict()
    {
        var fleet = await NewFleetAsync();
        var existing = await InstalledAsync(fleet.EngineOne, fleet.AircraftId, 1, September24);

        using var response = await _fleet.InstallAsync(fleet.EngineTwo, fleet.AircraftId, 1, October10);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var error = await FirstErrorOf(response);
        Assert.Equal(80211, error.GetProperty("code").GetInt32());

        var message = error.GetProperty("title").GetString()!;
        Assert.Contains(FleetApi.IdOf(existing), message);
        Assert.Contains(fleet.SerialOfEngineOne, message);
        Assert.Contains(fleet.RegistrationOfAircraft, message);
        Assert.Contains("position 1", message);
        Assert.Contains("from 2026-09-24T00:00:00Z to open", message);
    }

    [Fact]
    public async Task The_same_engine_cannot_be_installed_on_two_aircraft_at_once()
    {
        var fleet = await NewFleetAsync();
        var otherAircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(FleetApi.UniqueRegistration()));
        await InstalledAsync(fleet.EngineOne, fleet.AircraftId, 1, September24);

        using var response = await _fleet.InstallAsync(fleet.EngineOne, otherAircraftId, 2, October10);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(80211, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_removal_not_after_the_installation_is_refused()
    {
        var fleet = await NewFleetAsync();
        var installation = await InstalledAsync(fleet.EngineOne, fleet.AircraftId, 1, October10);

        using var response = await _fleet.PostAsync($"installations/{FleetApi.IdOf(installation)}/close", new { removedUtc = September24, reason = "removed" });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80212, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Closing_twice_is_refused()
    {
        var fleet = await NewFleetAsync();
        var installation = await InstalledAsync(fleet.EngineOne, fleet.AircraftId, 1, September24);
        await CloseAsync(FleetApi.IdOf(installation), October10);

        using var response = await _fleet.PostAsync($"installations/{FleetApi.IdOf(installation)}/close", new { removedUtc = October10.AddDays(1), reason = "again" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(80215, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Position_three_is_refused()
    {
        var fleet = await NewFleetAsync();

        using var response = await _fleet.InstallAsync(fleet.EngineOne, fleet.AircraftId, 3, September24);

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80209, await ApiResponses.FirstErrorCodeOf(response));
    }

    private async Task<JsonElement> InstalledAsync(string engineId, string aircraftId, int position, DateTimeOffset installedUtc)
    {
        using var response = await _fleet.InstallAsync(engineId, aircraftId, position, installedUtc);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    private async Task<JsonElement> CloseAsync(string installationId, DateTimeOffset removedUtc)
    {
        using var response = await _fleet.PostAsync($"installations/{installationId}/close", new { removedUtc, reason = "acceptance test removal" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    private static async Task<JsonElement> FirstErrorOf(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        return body.GetProperty("errors")[0];
    }

    private async Task<TestFleet> NewFleetAsync()
    {
        var registration = FleetApi.UniqueRegistration();
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(registration));
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));

        var serialOne = FleetApi.UniqueCode("ESN");
        var serialTwo = FleetApi.UniqueCode("ESN");
        var engineOne = FleetApi.IdOf(await _fleet.CreateEngineAsync(serialOne, engineModelId));
        var engineTwo = FleetApi.IdOf(await _fleet.CreateEngineAsync(serialTwo, engineModelId));

        return new TestFleet(aircraftId, registration, engineOne, serialOne, engineTwo, serialTwo);
    }

    private sealed record TestFleet(
        string AircraftId,
        string RegistrationOfAircraft,
        string EngineOne,
        string SerialOfEngineOne,
        string EngineTwo,
        string SerialOfEngineTwo);
}
