using System.Text.Json;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Quality;

public sealed class QualityGoldenTests
{
    private const string MeasuredCorpus = "reports/P04/quality-corpus.json is required: run stage/P04/evidence/quality_corpus.py (the pack oracle over samples/).";

    public static TheoryData<string> RealFiles()
    {
        var data = new TheoryData<string>();

        foreach (var fileName in Recordings.RealFiles)
            data.Add(fileName);

        return data;
    }

    [Theory]
    [MemberData(nameof(RealFiles))]
    public void The_quality_section_equals_the_golden(string fileName)
    {
        var golden = Path.ChangeExtension(fileName, ".oracle.json");
        using var document = JsonDocument.Parse(File.ReadAllBytes(RepositoryPaths.Fixtures("golden", golden)));

        var result = QualityParity.Evaluate(fileName, Recordings.RealBytes(fileName));

        QualityParity.AssertMatchesOracle(document.RootElement.GetProperty("quality"), result, fileName);
    }

    [Fact]
    public void Flight_24_shows_the_confirmed_jump_the_lag_and_the_clock_counters()
    {
        var result = QualityParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));
        var jump = Assert.Single(result.Discontinuities);
        var mask = result.Spans.Single(span => span.Code == QualitySpanCodes.AltitudeRateUntrusted);

        Assert.Equal((3887, QualitySpanCodes.DiscontinuityConfirmed), (jump.Index, jump.Code));
        Assert.Equal((3867, 3917), (mask.StartIndex, mask.EndIndexInclusive));
        Assert.Equal(["PRESSURE_ALT_FT", "RADIO_HEIGHT_FT"], mask.ParameterKeys);
        Assert.Equal(663, mask.Actual);
        Assert.Equal(300, mask.Threshold);
        Assert.Equal(["PHASE_ALTITUDE_RATE"], mask.Affects);
        Assert.Equal(57, result.AirGroundLagSec);
        Assert.Equal((3987, 3986), (result.Clock!.InvalidOrMissingRows, result.Clock.SecondaryNotInStepRows));
        Assert.Equal(TimeTrust.UNTRUSTED, result.Clock.TimeTrust);
    }

    [Fact]
    public void The_seed_rules_grade_the_real_data_as_measured_and_find_no_stuck_sensor()
    {
        var files = Directory.GetFiles(Path.Combine(RepositoryPaths.Root, "samples"), "*.csv", SearchOption.AllDirectories);
        var results = files.Select(path => QualityParity.EvaluateWithSeed(Path.GetFileName(path), File.ReadAllBytes(path))).ToList();
        var grades = results.GroupBy(result => result.Grade).ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(new Dictionary<QualityGrade, int> { [QualityGrade.A] = 70, [QualityGrade.B] = 1, [QualityGrade.C] = 31 }, grades);
        Assert.DoesNotContain(results.SelectMany(result => result.Spans), span => span.Code == QualitySpanCodes.SensorStuck);
        Assert.All(results, result => Assert.Equal(0, result.Clock!.InvalidOrMissingRows));
    }

    [Fact]
    public void Every_real_file_in_samples_matches_the_oracle()
    {
        var corpusPath = Path.Combine(RepositoryPaths.Root, "reports", "P04", "quality-corpus.json");

        Assert.True(File.Exists(corpusPath), MeasuredCorpus);

        using var corpus = JsonDocument.Parse(File.ReadAllBytes(corpusPath));
        var oracleByFile = corpus.RootElement.GetProperty("flights").EnumerateArray()
            .ToDictionary(flight => flight.GetProperty("file").GetString()!);
        var files = Directory.GetFiles(Path.Combine(RepositoryPaths.Root, "samples"), "*.csv", SearchOption.AllDirectories);

        Assert.Equal(oracleByFile.Count, files.Length);

        foreach (var path in files)
        {
            var fileName = Path.GetFileName(path);
            var result = QualityParity.Evaluate(fileName, File.ReadAllBytes(path));

            QualityParity.AssertMatchesOracle(oracleByFile[fileName], result, fileName);
        }
    }
}
