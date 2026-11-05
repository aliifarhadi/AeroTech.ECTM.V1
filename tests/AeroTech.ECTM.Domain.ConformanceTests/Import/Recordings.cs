using System.Text;
using System.Text.Json;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Configuration;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public static class Recordings
{
    public const string Flight24 = "2026-10-01_EP-DOA_24.csv";
    public const string Flight25 = "2026-10-01_EP-DOA_25.csv";

    private const long PartLimit = 50_000_000;

    public static readonly string[] RealFiles =
    [
        "2026-09-24_EP-DOA_9072.csv",
        "2026-09-30_EP-DOA_36.csv",
        "2026-09-30_EP-DOA_37.csv",
        "2026-09-30_EP-DOA_6950.csv",
        "2026-09-30_EP-DOA_6951.csv",
        Flight24,
        Flight25,
        "2026-10-01_EP-DOA_26.csv"
    ];

    public static DataProfileContent Profile { get; } = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true).Profile!;

    public static ValidatedCatalog Catalog { get; } = SeedDocuments.ValidatedCatalog;

    public static byte[] RealBytes(string fileName) => File.ReadAllBytes(RepositoryPaths.Fixtures("real", fileName));

    public static ParsedRecording Parse(string fileName, byte[] content)
        => RecordingReader.Read(Profile, Catalog, SourceRows.Read(fileName, content, ',', PartLimit));

    public static ParsedRecording ParseReal(string fileName) => Parse(fileName, RealBytes(fileName));

    public static JsonElement GoldenIdentity(string fileName)
    {
        var golden = Path.ChangeExtension(fileName, ".oracle.json");
        using var document = JsonDocument.Parse(File.ReadAllBytes(RepositoryPaths.Fixtures("golden", golden)));

        return document.RootElement.GetProperty("identity").Clone();
    }

    public static RecordedChannel Channel(ParsedRecording recording, string parameterKey)
        => recording.Channels.Single(channel => channel.ParameterKey == parameterKey);

    public static byte[] WithLines(byte[] content, Func<List<string>, List<string>> change)
    {
        var text = Encoding.UTF8.GetString(content);
        var lines = text.Split("\r\n").ToList();
        var changed = change(lines);

        return Encoding.UTF8.GetBytes(string.Join("\r\n", changed));
    }

    public static string WithCell(string line, int ordinal, string value)
    {
        var cells = line.Split(',');
        cells[ordinal] = value;

        return string.Join(',', cells);
    }

    public static int OrdinalOf(string parameterKey)
        => Profile.Columns!.Single(column => DataProfileValidator.KeyOf(column.ParameterCode!, column.Position) == parameterKey).Ordinal!.Value;
}
