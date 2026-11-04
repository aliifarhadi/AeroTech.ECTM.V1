using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fleet;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class EngineTests(MigratedDatabaseFixture fixture)
{
    private static readonly DateTimeOffset Basis = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);

    private readonly FleetApi _fleet = new(fixture.Client);

    [Fact]
    public async Task An_engine_registered_with_counters_has_one_initial_adjustment()
    {
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));
        var serialNumber = FleetApi.UniqueCode("ESN");

        var engine = await _fleet.CreatedAsync("engines", new
        {
            serialNumber,
            engineModelId,
            status = "ACTIVE",
            lifeCounters = new { hoursSinceNew = 1000.5m, cyclesSinceNew = 500, basisUtc = Basis }
        });

        Assert.Equal(serialNumber, engine.GetProperty("serialNumber").GetString());
        Assert.Equal(FleetApi.Manufacturer, engine.GetProperty("manufacturer").GetString());
        Assert.Equal(1000.5m, engine.GetProperty("lifeCounters").GetProperty("hoursSinceNew").GetDecimal());
        Assert.Equal("MANUAL", engine.GetProperty("lifeCounters").GetProperty("source").GetProperty("name").GetString());

        var adjustment = Assert.Single(engine.GetProperty("lifeCounterAdjustments").EnumerateArray());
        Assert.Equal("Initial life counters at registration", adjustment.GetProperty("reason").GetString());
    }

    [Fact]
    public async Task Setting_life_counters_appends_an_adjustment_and_writes_the_audit()
    {
        var engineId = await NewEngineIdAsync();

        using var response = await _fleet.PutAsync($"engines/{engineId}/life-counters", new
        {
            hoursSinceNew = 2000m,
            cyclesSinceNew = 900,
            basisUtc = Basis.AddDays(10),
            reason = "logbook transcription"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var engine = await ApiResponses.DataOf(response);
        var adjustment = Assert.Single(engine.GetProperty("lifeCounterAdjustments").EnumerateArray());
        Assert.Equal("logbook transcription", adjustment.GetProperty("reason").GetString());
        Assert.Equal(900, engine.GetProperty("lifeCounters").GetProperty("cyclesSinceNew").GetInt32());

        var engineAudit = await AuditOf("Engine", engineId);
        var update = engineAudit.Single(entry => entry.GetProperty("action").GetProperty("name").GetString() == "UPDATED");
        Assert.Equal(JsonValueKind.Null, update.GetProperty("before").GetProperty("CyclesSinceNew").ValueKind);
        Assert.Equal(900, update.GetProperty("after").GetProperty("CyclesSinceNew").GetInt32());

        var adjustmentAudit = await AuditOf("LifeCounterAdjustment", adjustment.GetProperty("id").GetString()!);
        Assert.Equal("CREATED", Assert.Single(adjustmentAudit).GetProperty("action").GetProperty("name").GetString());
    }

    [Fact]
    public async Task A_negative_counter_is_refused()
    {
        var engineId = await NewEngineIdAsync();

        using var response = await _fleet.PutAsync($"engines/{engineId}/life-counters", new { hoursSinceNew = -1m, basisUtc = Basis, reason = "typo" });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80214, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Setting_counters_without_a_reason_is_rejected()
    {
        var engineId = await NewEngineIdAsync();

        using var response = await _fleet.PutAsync($"engines/{engineId}/life-counters", new { hoursSinceNew = 1m, basisUtc = Basis, reason = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task A_variant_without_evidence_is_refused()
    {
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));

        using var response = await _fleet.PostAsync("engines", new
        {
            serialNumber = FleetApi.UniqueCode("ESN"),
            engineModelId,
            status = "ACTIVE",
            variantCode = "-3B1"
        });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80213, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task Documenting_the_configuration_records_its_evidence()
    {
        var engineId = await NewEngineIdAsync();

        using var response = await _fleet.PutAsync($"engines/{engineId}", new
        {
            status = "ACTIVE",
            variantCode = "TEST-VARIANT",
            thrustRatingCode = "TEST-RATING",
            configurationEvidenceReference = "acceptance evidence"
        });

        var engine = await ApiResponses.DataOf(response);
        var evidence = engine.GetProperty("configurationEvidence");
        Assert.Equal("acceptance evidence", evidence.GetProperty("reference").GetString());
        Assert.Equal(fixture.Clock.Now, evidence.GetProperty("documentedUtc").GetDateTimeOffset());
    }

    [Fact]
    public async Task A_duplicate_serial_of_the_same_manufacturer_is_refused()
    {
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));
        var serialNumber = FleetApi.UniqueCode("ESN");
        await _fleet.CreateEngineAsync(serialNumber, engineModelId);

        using var response = await _fleet.PostAsync("engines", new { serialNumber, engineModelId, status = "ACTIVE" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal(80207, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task An_engine_of_an_unknown_model_is_refused()
    {
        using var response = await _fleet.PostAsync("engines", new { serialNumber = FleetApi.UniqueCode("ESN"), engineModelId = "1", status = "ACTIVE" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(80203, await ApiResponses.FirstErrorCodeOf(response));
    }

    private async Task<string> NewEngineIdAsync()
    {
        var engineModelId = FleetApi.IdOf(await _fleet.CreateEngineModelAsync(FleetApi.UniqueCode("MODEL")));
        var engine = await _fleet.CreateEngineAsync(FleetApi.UniqueCode("ESN"), engineModelId);

        return FleetApi.IdOf(engine);
    }

    private async Task<List<JsonElement>> AuditOf(string entityType, string entityId)
    {
        using var response = await _fleet.GetAsync($"audit?entityType={entityType}&entityId={entityId}");
        var page = await ApiResponses.DataOf(response);

        return page.GetProperty("results").EnumerateArray().ToList();
    }
}
