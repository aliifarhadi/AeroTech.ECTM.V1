using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fleet;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class MaintenanceEventTests(MigratedDatabaseFixture fixture)
{
    private static readonly DateTimeOffset October5 = new(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);

    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task A_wash_on_an_engine_is_listed_with_its_serial_number()
    {
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));
        var serialNumber = FleetApi.UniqueCode("ESN");
        var engineId = FleetApi.IdOf(await _fleet.CreateEngineAsync(serialNumber, engineModelId));

        var wash = await _fleet.CreatedAsync("maintenance-events", new
        {
            engineId,
            eventType = "WASH",
            eventUtc = October5,
            dateReliability = "DAY_ONLY",
            hoursAtEvent = 12400m,
            workOrderReference = "WO-TEST-1"
        });

        Assert.Equal("ENGINE", wash.GetProperty("identityKind").GetProperty("name").GetString());
        Assert.Equal(serialNumber, wash.GetProperty("engineSerialNumber").GetString());

        using var listed = await _fleet.GetAsync($"maintenance-events?engineId={engineId}");
        var events = (await ApiResponses.DataOf(listed)).GetProperty("results");
        Assert.Equal(FleetApi.IdOf(wash), FleetApi.IdOf(Assert.Single(events.EnumerateArray())));
    }

    [Fact]
    public async Task A_position_only_event_names_the_aircraft_position_and_no_engine()
    {
        var registration = FleetApi.UniqueRegistration();
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(registration));

        var borescope = await _fleet.CreatedAsync("maintenance-events", new
        {
            aircraftId,
            position = 2,
            eventType = "BORESCOPE",
            eventUtc = October5,
            dateReliability = "APPROXIMATE"
        });

        Assert.Equal("POSITION_ONLY", borescope.GetProperty("identityKind").GetProperty("name").GetString());
        Assert.Equal(JsonValueKind.Null, borescope.GetProperty("engineId").ValueKind);
        Assert.Equal(registration, borescope.GetProperty("aircraftRegistration").GetString());
        Assert.Equal(2, borescope.GetProperty("position").GetInt32());

        using var listed = await _fleet.GetAsync($"maintenance-events?aircraftId={aircraftId}&position=2");
        Assert.Single((await ApiResponses.DataOf(listed)).GetProperty("results").EnumerateArray());

        using var updated = await _fleet.PutAsync($"maintenance-events/{FleetApi.IdOf(borescope)}", new
        {
            eventType = "BORESCOPE",
            eventUtc = October5,
            dateReliability = "EXACT",
            notes = "date confirmed"
        });
        var edited = await ApiResponses.DataOf(updated);
        Assert.Equal("EXACT", edited.GetProperty("dateReliability").GetProperty("name").GetString());
        Assert.Equal("date confirmed", edited.GetProperty("notes").GetString());
    }

    [Fact]
    public async Task An_event_naming_both_an_engine_and_a_position_is_refused()
    {
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(FleetApi.UniqueRegistration()));

        using var response = await _fleet.PostAsync("maintenance-events", new
        {
            engineId = "1",
            aircraftId,
            position = 1,
            eventType = "WASH",
            eventUtc = October5,
            dateReliability = "EXACT"
        });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80216, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task An_event_naming_neither_an_engine_nor_a_full_position_is_refused()
    {
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(FleetApi.UniqueRegistration()));

        using var response = await _fleet.PostAsync("maintenance-events", new
        {
            aircraftId,
            eventType = "WASH",
            eventUtc = October5,
            dateReliability = "EXACT"
        });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80216, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Negative_hours_are_refused()
    {
        var aircraftId = FleetApi.IdOf(await _fleet.CreateAircraftAsync(FleetApi.UniqueRegistration()));

        using var response = await _fleet.PostAsync("maintenance-events", new
        {
            aircraftId,
            position = 1,
            eventType = "WASH",
            eventUtc = October5,
            dateReliability = "EXACT",
            hoursAtEvent = -5m
        });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80217, await ApiResponses.FirstErrorCodeOf(response));
    }
}
