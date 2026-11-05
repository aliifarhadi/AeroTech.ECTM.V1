using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class SampleCorpusTests
{
    private const int MeasuredFileCount = 102;
    private const int MeasuredSampleCount = 409_837;
    private const string GapFlight = "2026-10-09_EP-DOA_47.csv";
    private const string Remeasure = "samples/ changed: re-run stage/P03/evidence/measure_samples.py and update stage/P03/GAP-ANALYSIS.md §5 before changing this test.";

    private static readonly string[] AlwaysEmpty = ["AT_N1_LIMIT_MODE", "FCC_N1_LIMIT_MODE", "DAA_N1_LIMIT_MODE"];

    [Fact]
    public void Every_real_file_in_samples_parses_with_the_seed_profile_as_measured()
    {
        var samples = Path.Combine(RepositoryPaths.Root, "samples");

        Assert.True(Directory.Exists(samples), "The real data folder samples/ is required for this test.");

        var files = Directory.GetFiles(samples, "*.csv", SearchOption.AllDirectories).Order(StringComparer.Ordinal).ToList();
        var sampleCount = 0;
        var gapFindings = new List<(string FileName, ValidationFinding Finding)>();

        Assert.True(files.Count == MeasuredFileCount, Remeasure);

        foreach (var path in files)
        {
            var fileName = Path.GetFileName(path);
            var recording = Recordings.Parse(fileName, File.ReadAllBytes(path));
            var claim = IdentityClaims.From(fileName, null, null, recording);
            var coverage = recording.Findings.Where(finding => finding.Code == ValidationCodes.ChannelCoverage).ToList();

            Assert.True(recording.SignatureMatches, fileName);
            Assert.True(recording.CountOf(ValidationSeverity.BLOCKING) == 0, fileName);
            Assert.True(claim.Reasons.Count == 0, $"{fileName}: {string.Join(", ", claim.Reasons)}");
            Assert.DoesNotContain(recording.Findings, finding => finding.Code is ValidationCodes.EnumLiteralOutsideDomain or ValidationCodes.RaggedRows);
            Assert.Equal(AlwaysEmpty.Order(), coverage.Select(finding => finding.ParameterKey!).Order());
            Assert.All(coverage, finding => Assert.Equal(0, finding.Value));

            sampleCount += recording.SampleCount;
            gapFindings.AddRange(recording.Findings.Where(finding => finding.Code == ValidationCodes.TimeAxisGap).Select(finding => (fileName, finding)));
        }

        Assert.True(sampleCount == MeasuredSampleCount, Remeasure);

        var (gapFile, gap) = Assert.Single(gapFindings);
        Assert.Equal(GapFlight, gapFile);
        Assert.Equal(ValidationSeverity.WARNING, gap.Severity);
        Assert.Equal(1, gap.Count);
        Assert.Equal(3, gap.Value);
    }

    [Fact]
    public void The_one_real_gap_is_after_sample_3942_of_flight_47()
    {
        var path = Directory.GetFiles(Path.Combine(RepositoryPaths.Root, "samples"), GapFlight, SearchOption.AllDirectories).Single();
        var recording = Recordings.Parse(GapFlight, File.ReadAllBytes(path));
        var axis = TimeAxis.Of(Recordings.Channel(recording, "ELAPSED_S").Numbers!);

        Assert.Equal(1, axis.NominalStep);
        Assert.Equal(new TimeGap(3942, 126669, 126673), Assert.Single(axis.Gaps));
    }
}
