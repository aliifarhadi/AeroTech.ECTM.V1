using System.Text.Json;
using AeroTech.ECTM.Domain.ConformanceTests.Import;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Phases;

public sealed class PhaseGoldenTests
{
    private const string MeasuredCorpus = "reports/P05/phase-corpus.json is required: run stage/P05/evidence/phase_corpus.py (the pack oracle over samples/).";

    public static TheoryData<string> RealFiles()
    {
        var data = new TheoryData<string>();

        foreach (var fileName in Recordings.RealFiles)
            data.Add(fileName);

        return data;
    }

    [Theory]
    [MemberData(nameof(RealFiles))]
    public void The_phase_section_equals_the_golden(string fileName)
    {
        var golden = Path.ChangeExtension(fileName, ".oracle.json");
        using var document = JsonDocument.Parse(File.ReadAllBytes(RepositoryPaths.Fixtures("golden", golden)));

        var result = PhaseParity.Evaluate(fileName, Recordings.RealBytes(fileName));

        PhaseParity.AssertMatchesOracle(document.RootElement.GetProperty("phase"), result, fileName);
    }

    [Theory]
    [MemberData(nameof(RealFiles))]
    public void Every_sample_is_in_exactly_one_segment(string fileName)
    {
        var result = PhaseParity.Evaluate(fileName, Recordings.RealBytes(fileName));
        var covered = result.Segments.Sum(segment => segment.EndIndexInclusive - segment.StartIndex + 1);

        Assert.Equal(result.SampleCount, covered);
        Assert.Equal(0, result.Segments[0].StartIndex);
        Assert.All(result.Segments.Zip(result.Segments.Skip(1)), pair => Assert.Equal(pair.First.EndIndexInclusive + 1, pair.Second.StartIndex));
    }

    [Theory]
    [MemberData(nameof(RealFiles))]
    public void Thrust_reverser_spans_cover_the_deployed_rows_of_the_golden(string fileName)
    {
        var expected = Recordings.GoldenIdentity(fileName).GetProperty("trDeployedRows");
        var result = PhaseParity.Evaluate(fileName, Recordings.RealBytes(fileName));

        foreach (var position in new[] { 1, 2 })
        {
            var deployed = result.Events
                .Where(span => span.Code == "THRUST_REVERSER_DEPLOYED" && span.Position == position)
                .Sum(span => span.EndIndexInclusive - span.StartIndex + 1);

            Assert.Equal(expected.GetProperty(position.ToString()).GetInt32(), deployed);
        }
    }

    [Fact]
    public void Flight_24_runs_from_cruise_to_the_landing_roll_without_a_late_climb()
    {
        var result = PhaseParity.Evaluate(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));
        var lastClimbEnd = result.Segments.Where(segment => segment.PhaseCode == FlightPhaseCode.CLIMB).Max(segment => segment.EndIndexInclusive);
        var phases = result.Segments.Select(segment => segment.PhaseCode).ToList();

        Assert.Contains(FlightPhaseCode.CRUISE, phases);
        Assert.Contains(FlightPhaseCode.LANDING_ROLL, phases);
        Assert.True(phases.LastIndexOf(FlightPhaseCode.CRUISE) < phases.IndexOf(FlightPhaseCode.LANDING_ROLL));
        Assert.True(lastClimbEnd < result.SampleCount - 120);
    }

    [Fact]
    public void Flight_24_explains_the_sample_before_the_jump_as_untrusted_altitude_rate()
    {
        var evaluation = PhaseParity.Run(Recordings.Flight24, Recordings.RealBytes(Recordings.Flight24));
        var explanation = evaluation.Explain(3886);
        var fired = explanation.Rules.Single(rule => rule.Fired);

        Assert.Equal(FlightPhaseCode.UNKNOWN, explanation.Label);
        Assert.Equal(PhaseUnknownReason.ALTITUDE_RATE_UNTRUSTED, explanation.UnknownReason);
        Assert.Equal("R-ALTITUDE_RATE_UNTRUSTED", fired.RuleId);
        Assert.Contains(explanation.Masks, mask => mask.Code == QualitySpanCodes.AltitudeRateUntrusted && mask.JumpIndex == 3887 && mask.StartIndex == 3867);
        Assert.Contains(explanation.Derived, value => value.Code == "ALT_RATE_SMOOTH_FPM" && value.Value is null && value.NullReason == QualitySpanCodes.AltitudeRateUntrusted);
    }

    [Fact]
    public void Every_real_file_in_samples_matches_the_oracle()
    {
        var corpusPath = Path.Combine(RepositoryPaths.Root, "reports", "P05", "phase-corpus.json");

        Assert.True(File.Exists(corpusPath), MeasuredCorpus);

        using var corpus = JsonDocument.Parse(File.ReadAllBytes(corpusPath));
        var oracleByFile = corpus.RootElement.GetProperty("flights").EnumerateArray()
            .ToDictionary(flight => flight.GetProperty("file").GetString()!);
        var files = Directory.GetFiles(Path.Combine(RepositoryPaths.Root, "samples"), "*.csv", SearchOption.AllDirectories);

        Assert.Equal(oracleByFile.Count, files.Length);

        foreach (var path in files)
        {
            var fileName = Path.GetFileName(path);
            var result = PhaseParity.Evaluate(fileName, File.ReadAllBytes(path));

            PhaseParity.AssertMatchesOracle(oracleByFile[fileName].GetProperty("phase"), result, fileName);
        }
    }
}
