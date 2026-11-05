using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(ImportDatabaseCollection.Name)]
public sealed class ImportWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string ArchiveName = "EP-DOA_eight_flights.zip";
    private const string Flight24 = "2026-10-01_EP-DOA_24.csv";
    private const string Flight25 = "2026-10-01_EP-DOA_25.csv";
    private const string AirGroundHeader = " AIR GROUND ()";

    private const string EgtOverN2 = """
        {"code":"EGT_OVER_N2","kind":"RATIO","role":"ENGINE_PERFORMANCE","unitOut":"CELSIUS_PER_PERCENT","status":"CANDIDATE",
         "description":"EGT per percent N2 (owner example)","numerator":"EGT_C[pos]","denominator":"N2_PCT[pos]","guardZero":true}
        """;

    private const string N1RangeForTheDryRun = """{"min":0,"max":90}""";

    private readonly ImportApi _imports = new(fixture);
    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task J3_eight_real_files_in_one_archive_become_eight_flights_pending_review_with_both_engines()
    {
        var fleet = await _imports.EnsureFleetAsync();
        await _imports.ActivateSeedsAsync();

        var archive = ImportApi.Zip(ImportApi.RealFiles.Select(name => (name, ImportApi.RealBytes(name))));
        var preview = await _imports.PreviewedAsync((ArchiveName, archive));
        var batchId = FleetApi.IdOf(preview);

        Assert.Equal("PREVIEWED", ImportApi.NameOf(preview, "state"));
        Assert.Equal(8, preview.GetProperty("items").GetArrayLength());

        foreach (var fileName in ImportApi.RealFiles)
        {
            var item = ImportApi.ItemNamed(preview, fileName);
            var identity = item.GetProperty("identity");

            Assert.Equal("NEW", ImportApi.NameOf(item, "outcome"));
            Assert.Equal(ArchiveName, item.GetProperty("containerName").GetString());
            Assert.Equal(ImportApi.GoldenRowsOf(fileName), item.GetProperty("sampleCount").GetInt32());
            Assert.Equal(0, item.GetProperty("blockingCount").GetInt32());
            Assert.Equal(10, item.GetProperty("firstRows").GetArrayLength());
            Assert.Equal(10, item.GetProperty("lastRows").GetArrayLength());
            Assert.Equal("RESOLVED", ImportApi.NameOf(identity, "identityStatus"));
            Assert.Equal([ImportApi.EngineOneSerial, ImportApi.EngineTwoSerial], ImportApi.SerialsOf(identity));
        }

        using (var commit = await _imports.CommitAsync(batchId, "j3-commit"))
        {
            Assert.Equal(HttpStatusCode.Accepted, commit.StatusCode);
            Assert.Equal("COMMITTING", ImportApi.NameOf(await ApiResponses.DataOf(commit), "state"));
        }

        var jobId = (await _imports.GetDataAsync($"imports/{batchId}")).GetProperty("jobId").GetString()!;

        using (var again = await _imports.CommitAsync(batchId, "j3-commit"))
            Assert.Equal(jobId, (await ApiResponses.DataOf(again)).GetProperty("jobId").GetString());

        await _imports.RunJobsAsync();

        var job = await _imports.GetDataAsync($"jobs/{jobId}");
        Assert.Equal("COMPLETED", ImportApi.NameOf(job, "state"));
        Assert.Equal((8, 8), (job.GetProperty("processed").GetInt32(), job.GetProperty("total").GetInt32()));

        var batch = await _imports.GetDataAsync($"imports/{batchId}");
        Assert.Equal("COMMITTED", ImportApi.NameOf(batch, "state"));

        foreach (var item in batch.GetProperty("items").EnumerateArray())
        {
            Assert.Equal("COMMITTED", ImportApi.NameOf(item, "state"));
            Assert.Equal("PENDING_REVIEW", ImportApi.NameOf(item, "flightState"));
            Assert.Equal("RESOLVED", ImportApi.NameOf(item.GetProperty("identity"), "identityStatus"));
            Assert.Equal([ImportApi.EngineOneSerial, ImportApi.EngineTwoSerial], ImportApi.SerialsOf(item.GetProperty("identity")));
        }

        using (var otherKey = await _imports.CommitAsync(batchId, "another-key"))
        {
            Assert.Equal(HttpStatusCode.Conflict, otherKey.StatusCode);
            Assert.Equal(80307, await ApiResponses.FirstErrorCodeOf(otherKey));
        }

        var flight24 = ImportApi.ItemNamed(batch, Flight24).GetProperty("flightId").GetString()!;
        var flight25 = ImportApi.ItemNamed(batch, Flight25).GetProperty("flightId").GetString()!;

        await TheSameBytesAgainAreLinkedNotImported(flight25);
        await TheRawChartShowsBothEnginesOfFlight24(flight24);
        await Flight25MatchesTheIndependentValues(flight25);
        await TheFlightListFiltersByEngine(fleet.EngineOneId);
        await HoldAndReleaseFollowTheStateMachine(flight24);
        await ACatalogDraftDryRunsOnFlight25(flight25);
    }

    private async Task TheSameBytesAgainAreLinkedNotImported(string flight25)
    {
        var preview = await _imports.PreviewedAsync((Flight25, ImportApi.RealBytes(Flight25)));
        var item = ImportApi.ItemNamed(preview, Flight25);

        Assert.Equal("DUPLICATE", ImportApi.NameOf(item, "outcome"));
        Assert.Equal(flight25, item.GetProperty("existingFlightId").GetString());
        Assert.Equal(0, item.GetProperty("sampleCount").GetInt32());

        var committed = await _imports.CommittedAsync(FleetApi.IdOf(preview), "duplicate-commit");
        var committedItem = ImportApi.ItemNamed(committed, Flight25);

        Assert.Equal("SKIPPED", ImportApi.NameOf(committedItem, "state"));
        Assert.Equal(flight25, committedItem.GetProperty("flightId").GetString());

        var flights = await _imports.GetDataAsync("flights?pageSize=50");
        Assert.Equal("8", flights.GetProperty("totalCount").GetString());
    }

    private async Task TheRawChartShowsBothEnginesOfFlight24(string flight24)
    {
        var series = await _imports.GetDataAsync($"flights/{flight24}/series?parameters=N1_PCT[1],N1_PCT[2],EGT_C[1],EGT_C[2]&budget=400");
        var parameters = series.GetProperty("parameters").EnumerateArray().ToList();

        Assert.Equal(["N1_PCT[1]", "N1_PCT[2]", "EGT_C[1]", "EGT_C[2]"], parameters.Select(parameter => parameter.GetProperty("parameterKey").GetString()));
        Assert.All(parameters, parameter =>
        {
            Assert.Equal("M4", parameter.GetProperty("sampling").GetString());
            Assert.Equal(ImportApi.GoldenRowsOf(Flight24), parameter.GetProperty("sampleCountInRange").GetInt32());
            Assert.InRange(parameter.GetProperty("points").GetArrayLength(), 4, 400);
        });

        using var unknown = await _imports.GetAsync($"flights/{flight24}/series?parameters=NOT_A_CHANNEL");
        Assert.Equal(HttpStatusCode.UnprocessableEntity, unknown.StatusCode);
        Assert.Equal(80316, await ApiResponses.FirstErrorCodeOf(unknown));
    }

    private async Task Flight25MatchesTheIndependentValues(string flight25)
    {
        var series = await _imports.GetDataAsync($"flights/{flight25}/series?parameters=N1_PCT[1],EGT_C[2]&sampling=RAW&from=101&to=121");
        var n1 = series.GetProperty("parameters")[0].GetProperty("points");
        var egt = series.GetProperty("parameters")[1].GetProperty("points");

        Assert.Equal(21, n1.GetArrayLength());
        Assert.Equal((101, 114960, 92.52), PointOf(n1[0]));
        Assert.Equal((121, 114980, 92.06), PointOf(n1[20]));
        Assert.Equal((101, 114960, 873), PointOf(egt[0]));
        Assert.Equal((121, 114980, 872), PointOf(egt[20]));

        var samples = await _imports.GetDataAsync($"flights/{flight25}/samples?from=101&to=121");
        var header = samples.GetProperty("header").EnumerateArray().Select(cell => cell.GetString()!).ToList();
        var rows = samples.GetProperty("rows");
        var airGround = header.IndexOf(AirGroundHeader);

        Assert.Equal(85, header.Count);
        Assert.Equal(21, rows.GetArrayLength());
        Assert.Equal(101, rows[0].GetProperty("sampleIndex").GetInt32());
        Assert.Equal(" GND", rows[0].GetProperty("cells")[airGround].GetString());
        Assert.Equal(" AIR", rows[20].GetProperty("cells")[airGround].GetString());

        var detail = await _imports.GetDataAsync($"flights/{flight25}");
        Assert.Equal(ImportApi.GoldenRowsOf(Flight25), detail.GetProperty("sampleCount").GetInt32());
        Assert.Equal(85, detail.GetProperty("channels").GetArrayLength());
        Assert.Equal("REAL_SOURCE", ImportApi.NameOf(detail.GetProperty("sourceFile"), "sourceType"));
    }

    private async Task TheFlightListFiltersByEngine(string engineOneId)
    {
        var byEngine = await _imports.GetDataAsync($"flights?engineId={engineOneId}&state=PENDING_REVIEW&pageSize=50");

        Assert.Equal("8", byEngine.GetProperty("totalCount").GetString());
    }

    private async Task HoldAndReleaseFollowTheStateMachine(string flight24)
    {
        using (var hold = await _imports.PostAsync($"flights/{flight24}/review", new { action = "HOLD", note = "waiting for the shop visit report" }))
            Assert.Equal("HELD", ImportApi.NameOf(await ApiResponses.DataOf(hold), "state"));

        using (var release = await _imports.PostAsync($"flights/{flight24}/review", new { action = "RELEASE" }))
            Assert.Equal("PENDING_REVIEW", ImportApi.NameOf(await ApiResponses.DataOf(release), "state"));

        using (var accept = await _imports.PostAsync($"flights/{flight24}/review", new { action = "ACCEPT" }))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, accept.StatusCode);
            Assert.Equal(80315, await ApiResponses.FirstErrorCodeOf(accept));
        }

        using (var releaseAgain = await _imports.PostAsync($"flights/{flight24}/review", new { action = "RELEASE" }))
        {
            Assert.Equal(HttpStatusCode.Conflict, releaseAgain.StatusCode);
            Assert.Equal(80314, await ApiResponses.FirstErrorCodeOf(releaseAgain));
        }
    }

    private async Task ACatalogDraftDryRunsOnFlight25(string flight25)
    {
        var activeCatalogId = FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.Catalog));
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.Catalog, new { basedOnDocumentId = activeCatalogId }));
        var content = await _configuration.ContentOfAsync(ConfigurationApi.Catalog, draftId);
        var n1 = content["parameters"]!.AsArray().Single(parameter => parameter!["code"]!.GetValue<string>() == "N1_PCT")!;

        content["derivedParameters"]!.AsArray().Add(JsonNode.Parse(EgtOverN2));
        n1["plausibility"] = JsonNode.Parse(N1RangeForTheDryRun);

        using (var edited = await _configuration.PutAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}", new { content }))
            Assert.Equal(HttpStatusCode.OK, edited.StatusCode);

        using var dryRun = await _imports.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/dry-run/flight", new { flightId = flight25, budget = 20_000 });
        Assert.Equal(HttpStatusCode.OK, dryRun.StatusCode);

        var result = await ApiResponses.DataOf(dryRun);
        var derived = result.GetProperty("derivedParameters").EnumerateArray().ToDictionary(series => series.GetProperty("parameterKey").GetString()!);
        var egtOverN2 = derived["EGT_OVER_N2[1]"];
        var n1Plausibility = result.GetProperty("findings").EnumerateArray()
            .Single(finding => finding.GetProperty("code").GetString() == "PLAUSIBILITY_OUT_OF_RANGE" && finding.GetProperty("parameterKey").GetString() == "N1_PCT[1]");

        Assert.Empty(result.GetProperty("configurationIssues").EnumerateArray());
        Assert.Equal(activeCatalogId, result.GetProperty("activeDocumentId").GetString());
        Assert.Equal("ADDED", egtOverN2.GetProperty("againstActive").GetString());
        Assert.Equal(ImportApi.GoldenRowsOf(Flight25), egtOverN2.GetProperty("availableCount").GetInt32());
        Assert.Equal((101, 114960, 848 / 97.85), PointOf(egtOverN2.GetProperty("points")[101]));
        Assert.Equal("UNCHANGED", derived["EGT_DIFF_E2_E1_C"].GetProperty("againstActive").GetString());
        Assert.Equal("ADDED", n1Plausibility.GetProperty("change").GetString());
        Assert.Empty(result.GetProperty("removedFromActive").EnumerateArray());

        var profileId = result.GetProperty("flightVersions").GetProperty("dataProfileDocumentId").GetString();

        using (var profile = await _imports.PostAsync($"config/{ConfigurationApi.Profile}/documents/{profileId}/dry-run/flight", new { flightId = flight25 }))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, profile.StatusCode);
            Assert.Equal(80111, await ApiResponses.FirstErrorCodeOf(profile));
        }

        using (var unknownFlight = await _imports.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{draftId}/dry-run/flight", new { flightId = "987654321" }))
        {
            Assert.Equal(HttpStatusCode.NotFound, unknownFlight.StatusCode);
            Assert.Equal(80311, await ApiResponses.FirstErrorCodeOf(unknownFlight));
        }
    }

    private static (int, double, double) PointOf(JsonElement point)
        => (point.GetProperty("sampleIndex").GetInt32(), point.GetProperty("elapsed").GetDouble(), point.GetProperty("value").GetDouble());
}
