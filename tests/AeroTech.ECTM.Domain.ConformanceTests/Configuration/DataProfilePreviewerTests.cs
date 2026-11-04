using System.Text;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class DataProfilePreviewerTests
{
    private const string Flight24 = "2026-10-01_EP-DOA_24.csv";
    private const int Flight24Rows = 3987;
    private const double Flight24FirstElapsed = 99095;
    private const double Flight24LastElapsed = 103081;

    private static readonly DataProfileContent Profile = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true).Profile!;

    [Fact]
    public void A_real_recording_matches_the_profile_signature_and_previews_both_ends()
    {
        var preview = PreviewOf(File.ReadAllBytes(RepositoryPaths.Fixtures("real", Flight24)));

        Assert.True(preview.SignatureMatches);
        Assert.Equal(Flight24Rows, preview.DataRowCount);
        Assert.Equal(0, preview.RaggedRowCount);
        Assert.Equal(10, preview.FirstRows.Count);
        Assert.Equal(10, preview.LastRows.Count);
        Assert.Equal(Flight24FirstElapsed, ElapsedOf(preview.FirstRows[0]));
        Assert.Equal(Flight24LastElapsed, ElapsedOf(preview.LastRows[^1]));
    }

    [Fact]
    public void Both_engine_positions_carry_the_same_engine_channels()
    {
        var preview = PreviewOf(File.ReadAllBytes(RepositoryPaths.Fixtures("real", Flight24)));

        var positions = preview.Engines.Select(engine => engine.Position).ToList();
        var engineOne = preview.Engines[0].ParameterKeys.Select(key => key.Replace("[1]", "")).Order();
        var engineTwo = preview.Engines[1].ParameterKeys.Select(key => key.Replace("[2]", "")).Order();

        Assert.Equal([1, 2], positions);
        Assert.Equal(engineOne, engineTwo);
        Assert.Contains("EGT_C[1]", preview.Engines[0].ParameterKeys);
        Assert.Contains("EGT_C[2]", preview.Engines[1].ParameterKeys);
    }

    [Fact]
    public void The_always_empty_columns_are_counted_as_missing_not_as_zero()
    {
        var preview = PreviewOf(File.ReadAllBytes(RepositoryPaths.Fixtures("real", Flight24)));

        foreach (var ordinal in new[] { 64, 77, 78 })
        {
            var summary = preview.ParseSummary.Single(column => column.Ordinal == ordinal);
            Assert.Equal(Flight24Rows, summary.MissingCount);
        }

        var cell = preview.FirstRows[0].Cells.Single(item => item.Ordinal == 64);
        Assert.Equal(DataProfilePreviewer.Missing, cell.Status);
        Assert.Null(cell.Number);
    }

    [Fact]
    public void A_swapped_header_is_a_signature_mismatch_and_nothing_is_parsed()
    {
        var bytes = File.ReadAllBytes(RepositoryPaths.Fixtures("real", Flight24));
        var text = Encoding.UTF8.GetString(bytes);
        var lineEnd = text.IndexOf('\n');
        var header = text[..lineEnd].Split(',');
        (header[11], header[12]) = (header[12], header[11]);
        var swapped = Encoding.UTF8.GetBytes(string.Join(',', header) + text[lineEnd..]);

        var preview = PreviewOf(swapped);

        Assert.False(preview.SignatureMatches);
        Assert.Equal([11, 12], preview.HeaderMismatches.Select(mismatch => mismatch.Ordinal));
        Assert.Empty(preview.FirstRows);
        Assert.Equal(0, preview.DataRowCount);
    }

    private static ProfilePreview PreviewOf(byte[] recording)
        => DataProfilePreviewer.Preview(Profile, SeedDocuments.ValidatedCatalog, CsvRows.Read(recording, ','));

    private static double ElapsedOf(PreviewRow row) => row.Cells.Single(cell => cell.ParameterKey == "ELAPSED_S").Number!.Value;
}
