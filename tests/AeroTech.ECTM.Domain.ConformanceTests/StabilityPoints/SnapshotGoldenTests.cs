using System.Text.Json;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.ConformanceTests.Phases;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.StabilityPoints;

public sealed class SnapshotGoldenTests
{
    private const string MeasuredCorpus = "reports/P06/snapshot-corpus.json is required: run stage/P06/evidence/snapshot_corpus.py (the pack oracle over samples/).";
    private const string PairedFixture = "docs/ECTM-v3.3-Product-Pack/ECTM-v3.3-Product-Pack/fixtures/independent/paired-joint-block-v1.json";

    [Theory]
    [MemberData(nameof(PhaseGoldenTests.RealFiles), MemberType = typeof(PhaseGoldenTests))]
    public void The_takeoff_and_cruise_sections_equal_the_golden(string fileName)
    {
        var golden = Path.ChangeExtension(fileName, ".oracle.json");
        using var document = JsonDocument.Parse(File.ReadAllBytes(RepositoryPaths.Fixtures("golden", golden)));

        var result = SnapshotParity.Evaluate(fileName, Recordings.RealBytes(fileName));

        SnapshotParity.AssertMatchesOracle(document.RootElement.GetProperty("snapshots"), result.Points, fileName);
    }

    [Fact]
    public void The_paired_values_equal_the_frozen_independent_fixture()
    {
        using var fixture = JsonDocument.Parse(File.ReadAllBytes(Path.Combine(RepositoryPaths.Root, PairedFixture)));

        foreach (var record in fixture.RootElement.GetProperty("records").EnumerateArray())
        {
            var fileName = record.GetProperty("flight").GetString()!;
            var selector = record.GetProperty("selector").GetString()!;
            var result = SnapshotParity.Evaluate(fileName, Recordings.RealBytes(fileName));
            var paired = result.Points.Cruise.Single(entry => entry.Selector == selector).Paired;
            var context = $"{fileName} {selector}";

            Assert.True(record.GetProperty("status").GetString() == paired.Availability.ToString(), $"{context}: status {paired.Availability} {paired.Reason}");
            Assert.True(record.GetProperty("matchedCount").GetInt32() == paired.MatchedSamples, $"{context}: matched {paired.MatchedSamples}");

            if (record.TryGetProperty("reason", out var reason))
            {
                Assert.True(reason.GetString() == paired.Reason?.ToString(), $"{context}: reason {paired.Reason}");
                continue;
            }

            Assert.True((record.GetProperty("jointStart").GetInt32(), record.GetProperty("jointEnd").GetInt32()) == (paired.JointStartIndex, paired.JointEndIndexInclusive), $"{context}: joint block");
            Assert.True((record.GetProperty("reportStart").GetInt32(), record.GetProperty("reportEnd").GetInt32()) == (paired.ReportStartIndex, paired.ReportEndIndexInclusive), $"{context}: report window");
            Assert.True(record.GetProperty("sumE2minusE1_C").GetDouble() == paired.SumDifference, $"{context}: sum {paired.SumDifference}");
            Assert.Equal(record.GetProperty("meanE2minusE1_C").GetDouble(), paired.MeanDifference!.Value, 6);
        }
    }

    [Fact]
    public void The_N1_peak_and_the_EGT_peak_are_different_observations()
    {
        var offsets = Recordings.RealFiles
            .SelectMany(fileName => SnapshotParity.Evaluate(fileName, Recordings.RealBytes(fileName)).Points.TakeoffEgtPeaks)
            .Select(peak => peak.OffsetFromN1PeakSec)
            .ToList();

        Assert.Equal(16, offsets.Count);
        Assert.Contains(offsets, offset => offset != 0);
    }

    [Fact]
    public void Flight_6951_has_both_engines_at_operator_300s_and_60_paired_samples()
    {
        var result = SnapshotParity.Evaluate("2026-09-30_EP-DOA_6951.csv", Recordings.RealBytes("2026-09-30_EP-DOA_6951.csv"));
        var selector = result.Points.Cruise.Single(entry => entry.Selector == "OPERATOR_300S");

        Assert.Equal(2, selector.AvailableEngineCount);
        Assert.Equal((SnapshotAvailability.AVAILABLE, 60), (selector.Paired.Availability, selector.Paired.MatchedSamples));
    }

    [Fact]
    public void Flight_6950_has_no_engine_1_block_at_operator_300s_and_no_paired_value()
    {
        var result = SnapshotParity.Evaluate("2026-09-30_EP-DOA_6950.csv", Recordings.RealBytes("2026-09-30_EP-DOA_6950.csv"));
        var selector = result.Points.Cruise.Single(entry => entry.Selector == "OPERATOR_300S");

        Assert.Equal(SnapshotReason.NO_ELIGIBLE_BLOCK, selector.Engines.Single(engine => engine.Position == 1).Reason);
        Assert.Equal(SnapshotAvailability.AVAILABLE, selector.Engines.Single(engine => engine.Position == 2).Availability);
        Assert.Equal(SnapshotReason.MISSING_ENGINE_WINDOW, selector.Paired.Reason);
    }

    [Fact]
    public void Every_real_file_in_samples_matches_the_oracle()
    {
        var corpusPath = Path.Combine(RepositoryPaths.Root, "reports", "P06", "snapshot-corpus.json");

        Assert.True(File.Exists(corpusPath), MeasuredCorpus);

        using var corpus = JsonDocument.Parse(File.ReadAllBytes(corpusPath));
        var oracleByFile = corpus.RootElement.GetProperty("flights").EnumerateArray()
            .ToDictionary(flight => flight.GetProperty("file").GetString()!);
        var files = Directory.GetFiles(Path.Combine(RepositoryPaths.Root, "samples"), "*.csv", SearchOption.AllDirectories);

        Assert.Equal(oracleByFile.Count, files.Length);

        foreach (var path in files)
        {
            var fileName = Path.GetFileName(path);
            var result = SnapshotParity.Evaluate(fileName, File.ReadAllBytes(path));

            SnapshotParity.AssertMatchesOracle(oracleByFile[fileName].GetProperty("snapshots"), result.Points, fileName);
        }
    }
}
