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
    private const string Flight9072 = "2026-09-24_EP-DOA_9072.csv";
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
        await QualityIsEvaluatedOnCommit(batch, flight24, fleet.AircraftId);
        await PhasesAreEvaluatedOnCommit(flight24, fleet.AircraftId);
        await APhaseDraftDryRunsWithoutBeingPublished(flight24, ImportApi.ItemNamed(batch, Flight9072).GetProperty("flightId").GetString()!);
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

    private async Task QualityIsEvaluatedOnCommit(JsonElement batch, string flight24, string aircraftId)
    {
        Assert.All(batch.GetProperty("items").EnumerateArray(), item => Assert.Equal(JsonValueKind.Object, item.GetProperty("grade").ValueKind));

        var quality = await _imports.GetDataAsync($"flights/{flight24}/quality");
        var jump = Assert.Single(quality.GetProperty("discontinuities").EnumerateArray());
        var mask = quality.GetProperty("spans").EnumerateArray().Single(span => span.GetProperty("code").GetString() == "ALTITUDE_RATE_UNTRUSTED");

        Assert.Equal("PROVISIONAL", ImportApi.NameOf(quality, "status"));
        Assert.Equal("C", ImportApi.NameOf(quality, "grade"));
        Assert.Equal(["PARTIAL_START_AND_END"], quality.GetProperty("gradeReasons").EnumerateArray().Select(reason => reason.GetString()));
        Assert.Equal((3887, "ALTITUDE_DISCONTINUITY_CONFIRMED"), (jump.GetProperty("index").GetInt32(), jump.GetProperty("code").GetString()));
        Assert.Equal((3867, 3917), (mask.GetProperty("startIndex").GetInt32(), mask.GetProperty("endIndexInclusive").GetInt32()));
        Assert.Equal(57, quality.GetProperty("airGroundLagSec").GetInt32());
        Assert.Equal("UNTRUSTED", ImportApi.NameOf(quality.GetProperty("clock"), "timeTrust"));
        Assert.Equal(0, quality.GetProperty("clock").GetProperty("invalidOrMissingRows").GetInt32());

        using (var sameVersions = await _imports.PostAsync($"flights/{flight24}/recompute", new { }))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, sameVersions.StatusCode);
            Assert.Equal(80321, await ApiResponses.FirstErrorCodeOf(sameVersions));
        }

        using (var preview = await _imports.PostAsync($"flights/{flight24}/recompute", new { dryRun = true }))
        {
            var recompute = await ApiResponses.DataOf(preview);

            Assert.True(recompute.GetProperty("dryRun").GetBoolean());
            Assert.Equal(0, recompute.GetProperty("preview").GetProperty("changedSamples").GetInt32());
            Assert.Equal("C", ImportApi.NameOf(recompute.GetProperty("preview"), "proposedGrade"));
        }

        var detail = await _imports.GetDataAsync($"flights/{flight24}");
        Assert.Equal("C", ImportApi.NameOf(detail, "grade"));

        var aircraft = await _imports.GetDataAsync($"quality/aircraft/{aircraftId}?from=2026-09-24&to=2026-10-01");
        var grades = aircraft.GetProperty("grades").EnumerateArray().ToDictionary(grade => ImportApi.NameOf(grade, "grade"), grade => grade.GetProperty("flights").GetInt32());

        Assert.Equal((8, 8), (aircraft.GetProperty("flights").GetInt32(), aircraft.GetProperty("evaluated").GetInt32()));
        Assert.Equal(8, grades.Values.Sum());
        Assert.Equal(8, aircraft.GetProperty("untrustedTime").GetInt32());
        Assert.Equal(8, aircraft.GetProperty("withDiscontinuity").GetInt32());
    }

    private async Task PhasesAreEvaluatedOnCommit(string flight24, string aircraftId)
    {
        var golden = ImportApi.GoldenOf(Flight24);
        var goldenPhase = golden.GetProperty("phase");
        var rows = ImportApi.GoldenRowsOf(Flight24);
        var phases = await _imports.GetDataAsync($"flights/{flight24}/phases");
        var anchors = phases.GetProperty("anchors").EnumerateArray().ToDictionary(anchor => anchor.GetProperty("code").GetString()!);
        var segments = phases.GetProperty("segments").EnumerateArray().ToList();
        var lastClimbEnd = segments.Where(segment => ImportApi.NameOf(segment, "phase") == "CLIMB").Max(segment => segment.GetProperty("endIndexInclusive").GetInt32());

        Assert.Equal(1, phases.GetProperty("revisionNumber").GetInt32());
        Assert.Equal("PROVISIONAL", ImportApi.NameOf(phases, "status"));
        Assert.Equal(("phase-v1.0.3", "MATCHES_REFERENCE"), (phases.GetProperty("reference").GetProperty("algorithm").GetString(), ImportApi.NameOf(phases.GetProperty("reference"), "match")));
        Assert.Equal(("CANDIDATE", "PENDING"), (ImportApi.NameOf(phases.GetProperty("ruleSet"), "calibrationStatus"), ImportApi.NameOf(phases.GetProperty("ruleSet"), "engineeringSignOff")));
        Assert.False(string.IsNullOrWhiteSpace(phases.GetProperty("ruleSet").GetProperty("description").GetString()));
        Assert.Equal(goldenPhase.GetProperty("liftoffBasis").GetString(), ImportApi.NameOf(anchors["LIFTOFF"], "basis"));
        Assert.Equal(goldenPhase.GetProperty("touchdownIndex").GetInt32(), anchors["TOUCHDOWN"].GetProperty("index").GetInt32());
        Assert.Equal(goldenPhase.GetProperty("segments").GetArrayLength(), segments.Count);
        Assert.Equal(rows, segments.Sum(segment => segment.GetProperty("endIndexInclusive").GetInt32() - segment.GetProperty("startIndex").GetInt32() + 1));
        Assert.Contains(segments, segment => ImportApi.NameOf(segment, "phase") == "LANDING_ROLL");
        Assert.True(lastClimbEnd < rows - 120);
        Assert.Equal(segments.Count, phases.GetProperty("transitions").GetArrayLength());

        var why = await _imports.GetDataAsync($"flights/{flight24}/phases/explain?sampleIndex=3886");

        Assert.Equal(("UNKNOWN", "ALTITUDE_RATE_UNTRUSTED"), (ImportApi.NameOf(why, "phase"), ImportApi.NameOf(why, "unknownReason")));
        Assert.Equal("R-ALTITUDE_RATE_UNTRUSTED", why.GetProperty("firedRuleId").GetString());
        Assert.Contains(why.GetProperty("masks").EnumerateArray(), mask => mask.GetProperty("jumpIndex").GetInt32() == 3887);

        using (var outside = await _imports.GetAsync($"flights/{flight24}/phases/explain?sampleIndex={rows}"))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, outside.StatusCode);
            Assert.Equal(80320, await ApiResponses.FirstErrorCodeOf(outside));
        }

        var events = await _imports.GetDataAsync($"flights/{flight24}/events");
        var deployedRows = golden.GetProperty("identity").GetProperty("trDeployedRows");
        var fire = events.GetProperty("coverage").EnumerateArray().Where(coverage => coverage.GetProperty("code").GetString() == "ENGINE_FIRE").ToList();

        foreach (var position in new[] { 1, 2 })
        {
            var deployed = events.GetProperty("spans").EnumerateArray()
                .Where(span => span.GetProperty("code").GetString() == "THRUST_REVERSER_DEPLOYED" && span.GetProperty("position").GetInt32() == position)
                .Sum(span => span.GetProperty("endIndexInclusive").GetInt32() - span.GetProperty("startIndex").GetInt32() + 1);

            Assert.Equal(deployedRows.GetProperty(position.ToString()).GetInt32(), deployed);
        }

        Assert.Equal(2, fire.Count);
        Assert.All(fire, coverage => Assert.Equal("UNVERIFIED_ENUM", ImportApi.NameOf(coverage, "evaluability")));

        var revision = Assert.Single((await _imports.GetDataAsync($"flights/{flight24}/revisions")).EnumerateArray());

        Assert.Equal((1, "IMPORT", "CURRENT"), (revision.GetProperty("number").GetInt32(), ImportApi.NameOf(revision, "trigger"), ImportApi.NameOf(revision, "status")));
        Assert.Equal(phases.GetProperty("analysisRevisionId").GetString(), FleetApi.IdOf(revision));
        Assert.Equal(golden.GetProperty("identity").GetProperty("sha256").GetString(), revision.GetProperty("sourceSha256").GetString());

        var rates = await _imports.GetDataAsync($"events/aircraft/{aircraftId}?from=2026-09-24&to=2026-10-01");
        var reverser = rates.GetProperty("events").EnumerateArray().Single(rate => rate.GetProperty("code").GetString() == "THRUST_REVERSER_DEPLOYED");

        Assert.Equal((8, 8, 8), (rates.GetProperty("flights").GetInt32(), rates.GetProperty("flightsWithPhases").GetInt32(), rates.GetProperty("landings").GetInt32()));
        Assert.Equal((8, 8, 100.0), (reverser.GetProperty("evaluatedFlights").GetInt32(), reverser.GetProperty("flightsWithEvent").GetInt32(), reverser.GetProperty("perHundredLandings").GetDouble()));
    }

    private async Task APhaseDraftDryRunsWithoutBeingPublished(string flight24, string flight9072)
    {
        var activeId = FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.PhaseRuleSet));
        var draftId = FleetApi.IdOf(await _configuration.CreateDraftAsync(ConfigurationApi.PhaseRuleSet, new { basedOnDocumentId = activeId }));
        var content = await _configuration.ContentOfAsync(ConfigurationApi.PhaseRuleSet, draftId);
        var cruise = content["conditions"]!.AsArray().Single(condition => condition!["code"]!.GetValue<string>() == "CRUISE")!;

        cruise["minSec"] = 240;

        using (var edited = await _configuration.PutAsync($"config/{ConfigurationApi.PhaseRuleSet}/documents/{draftId}", new { content }))
            Assert.Equal(HttpStatusCode.OK, edited.StatusCode);

        var shorterCruises = await DryRunPhasesAsync(draftId, flight9072);

        Assert.Equal(213 + 184, shorterCruises.GetProperty("changedSamples").GetInt32());
        Assert.All(shorterCruises.GetProperty("changes").EnumerateArray(), change =>
            Assert.Equal(("CRUISE", "LEVEL"), (ImportApi.NameOf(change, "before"), ImportApi.NameOf(change, "after"))));
        Assert.Equal("DRAFT", ImportApi.NameOf(shorterCruises, "proposedPhaseRuleSetStatus"));
        Assert.Equal(activeId, shorterCruises.GetProperty("currentPhaseRuleSetDocumentId").GetString());

        var longCruise = await DryRunPhasesAsync(draftId, flight24);

        Assert.Equal(0, longCruise.GetProperty("changedSamples").GetInt32());
        Assert.Equal(activeId, FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.PhaseRuleSet)));
        Assert.Equal(1, (await _imports.GetDataAsync($"flights/{flight9072}/phases")).GetProperty("revisionNumber").GetInt32());

        var catalogId = FleetApi.IdOf(await _configuration.ActiveVersionOfAsync(ConfigurationApi.Catalog));

        using var catalog = await _imports.PostAsync($"config/{ConfigurationApi.Catalog}/documents/{catalogId}/dry-run/phases", new { flightId = flight24 });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, catalog.StatusCode);
        Assert.Equal(80111, await ApiResponses.FirstErrorCodeOf(catalog));
    }

    private async Task<JsonElement> DryRunPhasesAsync(string draftId, string flightId)
    {
        using var response = await _imports.PostAsync($"config/{ConfigurationApi.PhaseRuleSet}/documents/{draftId}/dry-run/phases", new { flightId });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ApiResponses.DataOf(response);
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
