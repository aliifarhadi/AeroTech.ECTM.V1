using System.Security.Cryptography;
using System.Text.Json;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fixtures;

public sealed class FixtureIntegrityTests
{
    private const int RealRecordingCount = 8;
    private const int GoldenFileCount = 9;

    [Fact]
    public void Every_fixture_file_matches_the_manifest()
    {
        var manifest = ReadManifest();

        foreach (var (relativePath, expectedSha256) in manifest)
        {
            var actualSha256 = Sha256Of(RepositoryPaths.Fixtures(relativePath));

            Assert.Equal(expectedSha256, actualSha256);
        }

        Assert.Equal(RealRecordingCount + GoldenFileCount, manifest.Count);
    }

    [Fact]
    public void No_fixture_file_is_missing_from_the_manifest()
    {
        var manifest = ReadManifest();
        var filesOnDisk = FilesUnder("real").Concat(FilesUnder("golden")).Order(StringComparer.Ordinal).ToList();
        var filesInManifest = manifest.Keys.Order(StringComparer.Ordinal).ToList();

        Assert.Equal(filesInManifest, filesOnDisk);
    }

    [Fact]
    public void Every_real_recording_matches_the_identity_of_its_golden()
    {
        var goldenFiles = Directory.GetFiles(RepositoryPaths.Fixtures("golden"), "*.oracle.json");

        foreach (var goldenFile in goldenFiles)
        {
            using var golden = JsonDocument.Parse(File.ReadAllText(goldenFile));
            var identity = golden.RootElement.GetProperty("identity");
            var recordingName = identity.GetProperty("fileName").GetString()!;
            var expectedSha256 = identity.GetProperty("sha256").GetString()!;

            var actualSha256 = Sha256Of(RepositoryPaths.Fixtures("real", recordingName));

            Assert.Equal(expectedSha256, actualSha256);
        }

        Assert.Equal(RealRecordingCount, goldenFiles.Length);
    }

    private static Dictionary<string, string> ReadManifest()
    {
        var manifest = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var line in File.ReadAllLines(RepositoryPaths.Fixtures("MANIFEST.sha256")))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var digest = line[..64];
            var relativePath = line[66..];

            manifest.Add(relativePath, digest);
        }

        return manifest;
    }

    private static IEnumerable<string> FilesUnder(string folder)
        => Directory.GetFiles(RepositoryPaths.Fixtures(folder))
            .Select(path => $"{folder}/{Path.GetFileName(path)}");

    private static string Sha256Of(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexStringLower(hash);
    }
}
