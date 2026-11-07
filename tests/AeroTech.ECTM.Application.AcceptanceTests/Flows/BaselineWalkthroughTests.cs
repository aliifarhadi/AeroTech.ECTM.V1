using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(BaselineDatabaseCollection.Name)]
public sealed class BaselineWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string FirstPostShopFlight = "2026-09-24_EP-DOA_9072.csv";
    private const string LaterFlight = "2026-10-01_EP-DOA_26.csv";
    private const string Selector = "OPERATOR_60S";
    private const double Precision = 1e-9;

    private readonly ImportApi _imports = new(fixture);
    private readonly ConfigurationApi _configuration = new(fixture.Client);
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task An_engineer_prefills_from_one_file_edits_publishes_and_compares_later_flights()
    {
        var fleet = await _imports.EnsureFleetAsync();
        await _imports.ActivateSeedsAsync();
        await _configuration.EnsureActiveAsync("BASELINE_RULES");

        var prefill = await PrefillAsync(fleet.EngineOneId, FirstPostShopFlight, ImportApi.RealBytes(FirstPostShopFlight), 1);
        var goldenEgt = GoldenCruiseEgtOf(FirstPostShopFlight, "1");
        var suggestedEgt = SuggestionOf(prefill, "CRUISE", Selector, "EGT_C");

        Assert.Equal(goldenEgt, suggestedEgt, Precision);
        Assert.False(prefill.GetProperty("fileArchived").GetBoolean());
        Assert.Equal("0", (await _imports.GetDataAsync("flights")).GetProperty("totalCount").GetString());
        Assert.Contains(prefill.GetProperty("snapshots").EnumerateArray(), snapshot => ImportApi.NameOf(snapshot, "snapshotKind") == "TAKEOFF_EGT_PEAK_OBSERVED");

        var workbook = CsvWorkbook.FromCsv(ImportApi.RealBytes(FirstPostShopFlight));
        var prefillFromWorkbook = await PrefillAsync(fleet.EngineOneId, "first-after-shop.xlsx", workbook, 1);
        Assert.Equal(prefill.GetProperty("snapshots").GetRawText(), prefillFromWorkbook.GetProperty("snapshots").GetRawText());

        var engineTwoPrefill = await PrefillAsync(fleet.EngineOneId, FirstPostShopFlight, ImportApi.RealBytes(FirstPostShopFlight), 2);
        Assert.Equal(GoldenCruiseEgtOf(FirstPostShopFlight, "2"), SuggestionOf(engineTwoPrefill, "CRUISE", Selector, "EGT_C"), Precision);

        var cruise = SnapshotOf(prefill, "CRUISE", Selector);
        var draft = await SendAsync(HttpMethod.Post, $"engines/{fleet.EngineOneId}/baselines", HttpStatusCode.Created, new
        {
            name = "Post shop reference",
            declaredEffectiveDate = "2026-09-24",
            firstPostShopDeclared = true,
            sourceFileName = FirstPostShopFlight,
            sourceFileSha256 = prefill.GetProperty("sourceFileSha256").GetString(),
            parameters = new object[]
            {
                FromFile(cruise, "EGT_C", 661.0, suggestedEgt),
                FromFile(cruise, "N1_PCT", null, SuggestionOf(prefill, "CRUISE", Selector, "N1_PCT"))
            }
        });
        var draftId = FleetApi.IdOf(draft);

        Assert.Equal(["VALUE_MISSING"], draft.GetProperty("publicationIssues").EnumerateArray().Select(issue => issue.GetProperty("name").GetString()));
        Assert.Equal("FILE_EDITED", ImportApi.NameOf(ParameterOf(draft, "EGT_C"), "valueOrigin"));

        await ExpectErrorAsync(HttpMethod.Post, $"baselines/{draftId}/publish", HttpStatusCode.UnprocessableEntity, 80404, new { expectedDraftRevision = 1, confirmedRawReference = true });

        var revised = await SendAsync(HttpMethod.Put, $"baselines/{draftId}", HttpStatusCode.OK, new
        {
            expectedDraftRevision = 1,
            name = "Post shop reference",
            declaredEffectiveDate = "2026-09-24",
            firstPostShopDeclared = true,
            sourceFileName = FirstPostShopFlight,
            sourceFileSha256 = prefill.GetProperty("sourceFileSha256").GetString(),
            parameters = new object[] { FromFile(cruise, "EGT_C", 661.0, suggestedEgt), Manual("N2_PCT", 92.5) }
        });

        Assert.Equal(2, revised.GetProperty("draftRevision").GetInt32());
        Assert.Equal("MIXED", ImportApi.NameOf(revised, "inputProvenance"));
        await ExpectErrorAsync(HttpMethod.Put, $"baselines/{draftId}", HttpStatusCode.Conflict, 80403, new { expectedDraftRevision = 1, name = "stale", parameters = new object[] { Manual("N2_PCT", 92.5) } });
        await ExpectErrorAsync(HttpMethod.Post, $"baselines/{draftId}/publish", HttpStatusCode.BadRequest, null, new { expectedDraftRevision = 2, confirmedRawReference = false });

        var publication = await SendAsync(HttpMethod.Post, $"baselines/{draftId}/publish", HttpStatusCode.OK, new { expectedDraftRevision = 2, confirmedRawReference = true });
        Assert.Equal("PUBLISHED", ImportApi.NameOf(publication.GetProperty("baseline"), "state"));
        Assert.Equal("BASELINE_EVALUATION", ImportApi.NameOf(publication.GetProperty("evaluation"), "kind"));

        var manual = await SendAsync(HttpMethod.Post, $"engines/{fleet.EngineOneId}/baselines", HttpStatusCode.Created, new
        {
            name = "Engineering reference",
            parameters = new object[] { Manual("EGT_C", 675.0) }
        });
        var manualId = FleetApi.IdOf(manual);
        await SendAsync(HttpMethod.Post, $"baselines/{manualId}/publish", HttpStatusCode.OK, new { expectedDraftRevision = 1, confirmedRawReference = true });

        var manualEgtId = FleetApi.IdOf(ParameterOf(manual, "EGT_C"));
        await SendAsync(HttpMethod.Put, $"engines/{fleet.EngineOneId}/baselines/primary", HttpStatusCode.OK, new { baselineParameterId = manualEgtId });
        await ExpectErrorAsync(HttpMethod.Put, $"engines/{fleet.EngineOneId}/baselines/primary", HttpStatusCode.Conflict, 80408, new { baselineParameterId = FleetApi.IdOf(ParameterOf(revised, "EGT_C")) });

        var preview = await _imports.PreviewedAsync((FirstPostShopFlight, ImportApi.RealBytes(FirstPostShopFlight)), (LaterFlight, ImportApi.RealBytes(LaterFlight)));
        var batch = await _imports.CommittedAsync(FleetApi.IdOf(preview), "baseline-walkthrough");
        var sourceFlightId = ImportApi.ItemNamed(batch, FirstPostShopFlight).GetProperty("flightId").GetString()!;
        var laterFlightId = ImportApi.ItemNamed(batch, LaterFlight).GetProperty("flightId").GetString()!;

        await ReviewAsync(sourceFlightId, "ACCEPT");
        await ReviewAsync(laterFlightId, "ACCEPT");
        await _imports.RunJobsAsync();

        var comparisons = await EgtComparisonsOfAsync(laterFlightId);
        var laterEgt = GoldenCruiseEgtOf(LaterFlight, "1");

        Assert.Equal(2, comparisons.Count);
        Assert.Equal(laterEgt - 661.0, DeltaFor(comparisons, draftId), Precision);
        Assert.Equal(laterEgt - 675.0, DeltaFor(comparisons, manualId), Precision);
        Assert.All(comparisons, comparison =>
        {
            Assert.Equal("DESCRIPTIVE_RAW", ImportApi.NameOf(comparison, "status"));
            Assert.Equal(JsonValueKind.Null, comparison.GetProperty("correctedDelta").ValueKind);
            Assert.Contains("NO_APPROVED_CORRECTION_MODEL", ReasonsOf(comparison));
        });
        Assert.True(comparisons.Single(comparison => comparison.GetProperty("baselineId").GetString() == manualId).GetProperty("isPrimary").GetBoolean());

        var atSource = (await EgtComparisonsOfAsync(sourceFlightId)).Single(comparison => comparison.GetProperty("baselineId").GetString() == draftId);
        Assert.Equal(goldenEgt - 661.0, atSource.GetProperty("rawDelta").GetDouble(), Precision);
        Assert.Contains("REFERENCE_SOURCE_POINT", ReasonsOf(atSource));
        Assert.Contains("TEMPORAL_ORDER_UNVERIFIED", ReasonsOf(atSource));

        var trend = await _imports.GetDataAsync($"engines/{fleet.EngineOneId}/baseline-trend?baselineIds={draftId}&snapshotKind=CRUISE&selector={Selector}&parameterCode=EGT_C");
        Assert.Equal([sourceFlightId, laterFlightId], trend.GetProperty("results").EnumerateArray().Select(point => point.GetProperty("flightId").GetString()));

        await ReviewAsync(laterFlightId, "HOLD");
        Assert.All(await EgtComparisonsOfAsync(laterFlightId), comparison => Assert.Equal("NOT_FINAL", ImportApi.NameOf(comparison, "status")));
        await _imports.RunJobsAsync();
        Assert.Empty(await EgtComparisonsOfAsync(laterFlightId));

        await ReviewAsync(laterFlightId, "RELEASE");
        await ReviewAsync(laterFlightId, "ACCEPT");
        await _imports.RunJobsAsync();
        Assert.Equal(2, (await EgtComparisonsOfAsync(laterFlightId)).Count);

        var reconcile = await SendAsync(HttpMethod.Post, $"baselines/{draftId}/reconcile", HttpStatusCode.Accepted, new { });
        Assert.Equal("BASELINE_EVALUATION", ImportApi.NameOf(reconcile, "kind"));
        await _imports.RunJobsAsync();
        Assert.Equal(2, (await EgtComparisonsOfAsync(laterFlightId)).Count);

        var clone = await SendAsync(HttpMethod.Post, $"baselines/{draftId}/clone", HttpStatusCode.Created, new { });
        Assert.Equal((2, "DRAFT"), (clone.GetProperty("revisionNumber").GetInt32(), ImportApi.NameOf(clone, "state")));
        await ExpectErrorAsync(HttpMethod.Put, $"baselines/{draftId}", HttpStatusCode.Conflict, 80401, new { expectedDraftRevision = 2, name = "edit", parameters = new object[] { Manual("EGT_C", 662.0) } });
        Assert.Equal(661.0, ParameterOf(await _imports.GetDataAsync($"baselines/{draftId}"), "EGT_C").GetProperty("rawValue").GetDouble());

        await SendAsync(HttpMethod.Post, $"baselines/{manualId}/retire", HttpStatusCode.OK, new { reason = "replaced by the post-shop reference" });
        var engineBaselines = await _imports.GetDataAsync($"engines/{fleet.EngineOneId}/baselines");
        var parameters = engineBaselines.GetProperty("baselines").EnumerateArray().SelectMany(baseline => baseline.GetProperty("parameters").EnumerateArray());
        Assert.DoesNotContain(parameters, parameter => parameter.GetProperty("isPrimary").GetBoolean());
    }

    private async Task<JsonElement> PrefillAsync(string engineId, string fileName, byte[] content, int position)
    {
        using var form = new MultipartFormDataContent();
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(file, "file", fileName);
        form.Add(new StringContent(position.ToString()), "enginePosition");

        using var response = await _client.PostAsync($"Backoffice/v1/engines/{engineId}/baselines/prefill-file", form);

        Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        return await ApiResponses.DataOf(response);
    }

    private async Task<JsonElement> SendAsync(HttpMethod method, string route, HttpStatusCode expected, object body)
    {
        using var response = await _client.SendAsync(new HttpRequestMessage(method, $"Backoffice/v1/{route}") { Content = JsonContent.Create(body) });

        Assert.True(response.StatusCode == expected, $"{route}: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");

        return await ApiResponses.DataOf(response);
    }

    private async Task ExpectErrorAsync(HttpMethod method, string route, HttpStatusCode expected, int? code, object body)
    {
        using var response = await _client.SendAsync(new HttpRequestMessage(method, $"Backoffice/v1/{route}") { Content = JsonContent.Create(body) });

        Assert.True(response.StatusCode == expected, $"{route}: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");

        if (code is { } errorCode)
            Assert.Equal(errorCode, await ApiResponses.FirstErrorCodeOf(response));
    }

    private async Task ReviewAsync(string flightId, string action)
        => await SendAsync(HttpMethod.Post, $"flights/{flightId}/review", HttpStatusCode.OK, new { action, note = "baseline walkthrough" });

    private async Task<IReadOnlyList<JsonElement>> EgtComparisonsOfAsync(string flightId)
    {
        var comparisons = await _imports.GetDataAsync($"flights/{flightId}/baseline-comparisons?snapshotKind=CRUISE&selector={Selector}&parameterCode=EGT_C");

        return comparisons.GetProperty("evaluations").EnumerateArray().ToList();
    }

    private static double GoldenCruiseEgtOf(string fileName, string position)
        => ImportApi.GoldenOf(fileName)
            .GetProperty("snapshots").GetProperty("cruise").GetProperty(Selector)
            .GetProperty("engines").GetProperty(position)
            .GetProperty("windowMeans").GetProperty("egt").GetDouble();

    private static JsonElement SnapshotOf(JsonElement prefill, string kind, string? selector)
        => prefill.GetProperty("snapshots").EnumerateArray()
            .Single(snapshot => ImportApi.NameOf(snapshot, "snapshotKind") == kind && snapshot.GetProperty("selector").GetString() == selector);

    private static double SuggestionOf(JsonElement prefill, string kind, string? selector, string parameterCode)
        => SnapshotOf(prefill, kind, selector).GetProperty("values").EnumerateArray()
            .Single(value => value.GetProperty("parameterCode").GetString() == parameterCode)
            .GetProperty("suggestedRawValue").GetDouble();

    private static object FromFile(JsonElement snapshot, string parameterCode, double? rawValue, double suggestedRawValue)
    {
        var source = snapshot.GetProperty("source");

        return new
        {
            snapshotKind = ImportApi.NameOf(snapshot, "snapshotKind"),
            selector = snapshot.GetProperty("selector").GetString(),
            parameterCode,
            rawValue,
            suggestedRawValue,
            sourceEnginePosition = source.GetProperty("position").GetInt32(),
            sourceStartIndex = source.GetProperty("startIndex").GetInt32(),
            sourceEndIndexInclusive = source.GetProperty("endIndexInclusive").GetInt32(),
            referenceConditions = snapshot.GetProperty("referenceConditions"),
            referenceConfiguration = snapshot.GetProperty("referenceConfiguration").GetString()
        };
    }

    private static object Manual(string parameterCode, double rawValue)
        => new { snapshotKind = "CRUISE", selector = Selector, parameterCode, rawValue };

    private static JsonElement ParameterOf(JsonElement baseline, string parameterCode)
        => baseline.GetProperty("parameters").EnumerateArray().Single(parameter => parameter.GetProperty("parameterCode").GetString() == parameterCode);

    private static double DeltaFor(IReadOnlyList<JsonElement> comparisons, string baselineId)
        => comparisons.Single(comparison => comparison.GetProperty("baselineId").GetString() == baselineId).GetProperty("rawDelta").GetDouble();

    private static IReadOnlyList<string?> ReasonsOf(JsonElement comparison)
        => comparison.GetProperty("reasons").EnumerateArray().Select(reason => reason.GetProperty("name").GetString()).ToList();
}
