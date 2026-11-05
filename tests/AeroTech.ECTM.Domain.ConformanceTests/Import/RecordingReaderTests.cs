using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.SourceFileAggregate;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class RecordingReaderTests
{
    public static TheoryData<string> RealFiles()
    {
        var data = new TheoryData<string>();

        foreach (var fileName in Recordings.RealFiles)
            data.Add(fileName);

        return data;
    }

    [Theory]
    [MemberData(nameof(RealFiles))]
    public void Each_real_file_parses_to_its_golden_identity(string fileName)
    {
        var bytes = Recordings.RealBytes(fileName);
        var golden = Recordings.GoldenIdentity(fileName);
        var recording = Recordings.Parse(fileName, bytes);

        Assert.True(recording.SignatureMatches);
        Assert.Equal(golden.GetProperty("sha256").GetString(), SourceFile.Sha256Of(bytes));
        Assert.Equal(golden.GetProperty("rows").GetInt32(), recording.SampleCount);
        Assert.Equal(golden.GetProperty("firstElapsed").GetDouble(), recording.FirstElapsed);
        Assert.Equal(golden.GetProperty("lastElapsed").GetDouble(), recording.LastElapsed);
        Assert.Equal(TextsOf(golden, "flightNumberInFile"), recording.DistinctTextsOf(IdentityClaims.FlightNumberCode));
        Assert.Equal(TextsOf(golden, "dayInFile"), recording.DistinctTextsOf(IdentityClaims.DayCode));
        Assert.Equal(TextsOf(golden, "monthInFile"), recording.DistinctTextsOf(IdentityClaims.MonthCode));
        Assert.Equal(0, recording.CountOf(ValidationSeverity.BLOCKING));
    }

    [Theory]
    [InlineData(101, 114960, 92.52, 92.71, 97.85, 97.87, 848, 873, 6241, "GND")]
    [InlineData(121, 114980, 92.06, 90.68, 97.97, 97.33, 861, 872, 6009, "AIR")]
    public void Flight_25_matches_the_independent_values(int sample, double elapsed, double n1One, double n1Two, double n2One, double n2Two, double egtOne, double egtTwo, double fuelFlowOne, string airGround)
    {
        var recording = Recordings.ParseReal(Recordings.Flight25);

        Assert.Equal(elapsed, Recordings.Channel(recording, "ELAPSED_S").Numbers![sample]);
        Assert.Equal(n1One, Recordings.Channel(recording, "N1_PCT[1]").Numbers![sample]);
        Assert.Equal(n1Two, Recordings.Channel(recording, "N1_PCT[2]").Numbers![sample]);
        Assert.Equal(n2One, Recordings.Channel(recording, "N2_PCT[1]").Numbers![sample]);
        Assert.Equal(n2Two, Recordings.Channel(recording, "N2_PCT[2]").Numbers![sample]);
        Assert.Equal(egtOne, Recordings.Channel(recording, "EGT_C[1]").Numbers![sample]);
        Assert.Equal(egtTwo, Recordings.Channel(recording, "EGT_C[2]").Numbers![sample]);
        Assert.Equal(fuelFlowOne, Recordings.Channel(recording, "FUEL_FLOW_PPH[1]").Numbers![sample]);
        Assert.Equal(airGround, Recordings.Channel(recording, "AIR_GROUND").Texts![sample]);
    }

    [Fact]
    public void Swapped_columns_break_the_signature_and_no_sample_is_read()
    {
        var swapped = Recordings.WithLines(Recordings.RealBytes(Recordings.Flight24), lines =>
        {
            var cells = lines[0].Split(',');
            (cells[9], cells[10]) = (cells[10], cells[9]);
            lines[0] = string.Join(',', cells);
            return lines;
        });

        var recording = Recordings.Parse(Recordings.Flight24, swapped);

        Assert.False(recording.SignatureMatches);
        Assert.Equal([9, 10], recording.HeaderMismatches.Select(mismatch => mismatch.Ordinal));
        Assert.Equal(0, recording.SampleCount);
    }

    [Fact]
    public void Headers_are_compared_after_trimming()
    {
        var padded = Recordings.WithLines(Recordings.RealBytes(Recordings.Flight24), lines =>
        {
            var cells = lines[0].Split(',');
            cells[3] = $"   {cells[3].Trim()}   ";
            lines[0] = string.Join(',', cells);
            return lines;
        });

        Assert.True(Recordings.Parse(Recordings.Flight24, padded).SignatureMatches);
    }

    [Fact]
    public void A_repeated_or_decreasing_elapsed_value_is_blocking()
    {
        var elapsedOrdinal = Recordings.OrdinalOf("ELAPSED_S");
        var changed = Recordings.WithLines(Recordings.RealBytes(Recordings.Flight24), lines =>
        {
            var previous = lines[10].Split(',')[elapsedOrdinal];
            lines[11] = Recordings.WithCell(lines[11], elapsedOrdinal, previous);
            lines[21] = Recordings.WithCell(lines[21], elapsedOrdinal, "0");
            return lines;
        });

        var findings = Recordings.Parse(Recordings.Flight24, changed).Findings;

        Assert.Contains(findings, finding => finding is { Severity: ValidationSeverity.BLOCKING, Code: ValidationCodes.TimeAxisDuplicate, Count: 1 });
        Assert.Contains(findings, finding => finding is { Severity: ValidationSeverity.BLOCKING, Code: ValidationCodes.TimeAxisDecreasing, Count: 1 });
    }

    [Fact]
    public void A_ragged_row_is_a_warning_and_its_missing_cells_have_no_value()
    {
        var changed = Recordings.WithLines(Recordings.RealBytes(Recordings.Flight24), lines =>
        {
            var cells = lines[5].Split(',');
            lines[5] = string.Join(',', cells.Take(cells.Length - 1));
            return lines;
        });

        var recording = Recordings.Parse(Recordings.Flight24, changed);
        var lastKey = Recordings.Profile.Columns!.OrderBy(column => column.Ordinal).Last();
        var lastChannel = recording.Channels.Single(channel => channel.Ordinal == lastKey.Ordinal);

        Assert.Contains(recording.Findings, finding => finding is { Severity: ValidationSeverity.WARNING, Code: ValidationCodes.RaggedRows, Count: 1 });
        Assert.Null(lastChannel.Numbers?[4] ?? (object?)lastChannel.Texts?[4]);
    }

    [Fact]
    public void A_literal_outside_the_catalog_domain_is_a_warning_with_the_literal()
    {
        var airGround = Recordings.OrdinalOf("AIR_GROUND");
        var changed = Recordings.WithLines(Recordings.RealBytes(Recordings.Flight24), lines =>
        {
            lines[7] = Recordings.WithCell(lines[7], airGround, " TAXI");
            return lines;
        });

        var finding = Recordings.Parse(Recordings.Flight24, changed).Findings
            .Single(candidate => candidate.Code == ValidationCodes.EnumLiteralOutsideDomain);

        Assert.Equal(ValidationSeverity.WARNING, finding.Severity);
        Assert.Equal("AIR_GROUND", finding.ParameterKey);
        Assert.Equal(1, finding.Count);
        Assert.Contains("'TAXI' (1)", finding.Message);
    }

    [Fact]
    public void Coverage_and_plausibility_are_info_measurements_without_a_threshold()
    {
        var recording = Recordings.ParseReal(Recordings.Flight24);
        var info = recording.Findings.Where(finding => finding.Severity == ValidationSeverity.INFO).ToList();

        Assert.All(info, finding => Assert.Contains(finding.Code, new[] { ValidationCodes.ChannelCoverage, ValidationCodes.PlausibilityOutOfRange }));
        Assert.Contains(info, finding => finding is { Code: ValidationCodes.ChannelCoverage, ParameterKey: "AT_N1_LIMIT_MODE", Value: 0 });
    }

    private static List<string> TextsOf(System.Text.Json.JsonElement golden, string property)
        => golden.GetProperty(property).EnumerateArray().Select(item => item.GetString()!).ToList();
}
