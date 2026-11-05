using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class ImportApi(MigratedDatabaseFixture fixture)
{
    public const string Registration = "EP-DOA";
    public const string EngineOneSerial = "Eng1S1";
    public const string EngineTwoSerial = "Eng2S2";

    public static readonly DateTimeOffset InstalledOnSeptember24 = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);

    public static readonly string[] RealFiles =
    [
        "2026-09-24_EP-DOA_9072.csv",
        "2026-09-30_EP-DOA_36.csv",
        "2026-09-30_EP-DOA_37.csv",
        "2026-09-30_EP-DOA_6950.csv",
        "2026-09-30_EP-DOA_6951.csv",
        "2026-10-01_EP-DOA_24.csv",
        "2026-10-01_EP-DOA_25.csv",
        "2026-10-01_EP-DOA_26.csv"
    ];

    private readonly HttpClient _client = fixture.Client;

    public static byte[] RealBytes(string fileName) => File.ReadAllBytes(RepositoryPaths.Fixtures("real", fileName));

    public static int GoldenRowsOf(string fileName)
    {
        var golden = RepositoryPaths.Fixtures("golden", Path.ChangeExtension(fileName, ".oracle.json"));
        using var document = JsonDocument.Parse(File.ReadAllBytes(golden));

        return document.RootElement.GetProperty("identity").GetProperty("rows").GetInt32();
    }

    public static byte[] Zip(IEnumerable<(string Name, byte[] Content)> members)
    {
        using var buffer = new MemoryStream();

        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in members)
            {
                using var entry = archive.CreateEntry(name).Open();
                entry.Write(content);
            }
        }

        return buffer.ToArray();
    }

    public static byte[] WithLine(byte[] content, int lineIndex, Func<string, string> change)
    {
        var lines = Encoding.UTF8.GetString(content).Split("\r\n");
        lines[lineIndex] = change(lines[lineIndex]);

        return Encoding.UTF8.GetBytes(string.Join("\r\n", lines));
    }

    public async Task<FleetIds> EnsureFleetAsync()
    {
        var fleet = new FleetApi(_client);

        using var existing = await fleet.GetAsync($"aircraft?search={Registration}");
        var page = await ApiResponses.DataOf(existing);
        var found = page.GetProperty("results").EnumerateArray().FirstOrDefault();

        if (found.ValueKind == JsonValueKind.Object)
            return await FleetIdsOfAsync(FleetApi.IdOf(found));

        var aircraftId = FleetApi.IdOf(await fleet.CreateAircraftAsync(Registration));
        var engineModelId = FleetApi.IdOf(await fleet.CreateEngineModelAsync(FleetApi.UniqueCode("TEST-MODEL")));
        var engineOne = FleetApi.IdOf(await fleet.CreateEngineAsync(EngineOneSerial, engineModelId));
        var engineTwo = FleetApi.IdOf(await fleet.CreateEngineAsync(EngineTwoSerial, engineModelId));

        using (var first = await fleet.InstallAsync(engineOne, aircraftId, 1, InstalledOnSeptember24))
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        using (var second = await fleet.InstallAsync(engineTwo, aircraftId, 2, InstalledOnSeptember24))
            Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        return new FleetIds(aircraftId, engineOne, engineTwo);
    }

    public async Task ActivateSeedsAsync()
    {
        var configuration = new ConfigurationApi(_client);

        await configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        await configuration.EnsureActiveAsync(ConfigurationApi.Profile);
    }

    public Task<HttpResponseMessage> PreviewAsync(IEnumerable<(string Name, byte[] Content)> files, string? defaultRegistration = null, int? defaultYear = null)
    {
        var form = new MultipartFormDataContent();

        foreach (var (name, content) in files)
        {
            var file = new ByteArrayContent(content);
            file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(file, "files", name);
        }

        if (defaultRegistration is not null)
            form.Add(new StringContent(defaultRegistration), "defaultRegistration");

        if (defaultYear is not null)
            form.Add(new StringContent(defaultYear.Value.ToString()), "defaultYear");

        return _client.PostAsync("Backoffice/v1/imports/preview", form);
    }

    public async Task<JsonElement> PreviewedAsync(params (string Name, byte[] Content)[] files)
    {
        using var response = await PreviewAsync(files);

        Assert.True(response.StatusCode == HttpStatusCode.Created, await response.Content.ReadAsStringAsync());

        return await ApiResponses.DataOf(response);
    }

    public Task<HttpResponseMessage> CommitAsync(string batchId, string idempotencyKey)
        => _client.PostAsJsonAsync("Backoffice/v1/imports/commit", new { batchId, idempotencyKey });

    public async Task<JsonElement> CommittedAsync(string batchId, string idempotencyKey)
    {
        using (var response = await CommitAsync(batchId, idempotencyKey))
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        await RunJobsAsync();

        return await GetDataAsync($"imports/{batchId}");
    }

    public async Task RunJobsAsync()
    {
        var runner = fixture.Services.GetRequiredService<IProcessingJobRunner>();

        while (await runner.RunNextAsync())
        {
        }
    }

    public Task<HttpResponseMessage> GetAsync(string route) => _client.GetAsync($"Backoffice/v1/{route}");

    public Task<HttpResponseMessage> PostAsync(string route, object body) => _client.PostAsJsonAsync($"Backoffice/v1/{route}", body);

    public async Task<JsonElement> GetDataAsync(string route)
    {
        using var response = await GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ApiResponses.DataOf(response);
    }

    public static JsonElement ItemNamed(JsonElement batch, string fileName)
        => batch.GetProperty("items").EnumerateArray().Single(item => item.GetProperty("fileName").GetString() == fileName);

    public static string NameOf(JsonElement element, string property) => element.GetProperty(property).GetProperty("name").GetString()!;

    public static IReadOnlyList<string?> SerialsOf(JsonElement identity)
        => identity.GetProperty("engines").EnumerateArray()
            .Select(engine => engine.GetProperty("engineSerialNumber").GetString())
            .ToList();

    private async Task<FleetIds> FleetIdsOfAsync(string aircraftId)
    {
        var installations = await GetDataAsync($"installations?aircraftId={aircraftId}&openOnly=true");
        var engineIdByPosition = installations.GetProperty("results").EnumerateArray()
            .ToDictionary(installation => installation.GetProperty("position").GetInt32(), installation => installation.GetProperty("engineId").GetString()!);

        return new FleetIds(aircraftId, engineIdByPosition[1], engineIdByPosition[2]);
    }

    public sealed record FleetIds(string AircraftId, string EngineOneId, string EngineTwoId);
}
