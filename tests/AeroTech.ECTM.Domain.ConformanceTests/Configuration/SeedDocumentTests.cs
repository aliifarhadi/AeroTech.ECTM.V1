using System.Text.Json;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class SeedDocumentTests
{
    [Fact]
    public void The_catalog_seed_is_valid()
    {
        var validation = ParameterCatalogValidator.Validate(SeedDocuments.Catalog);

        Assert.Empty(validation.Issues);
        Assert.Equal(64 + 4, validation.Catalog!.Entries.Count);
    }

    [Fact]
    public void The_catalog_keeps_recorder_vibration_unverified()
    {
        var catalog = SeedDocuments.ValidatedCatalog;

        foreach (var code in new[] { "VIB_N1_FAN", "VIB_N2_HPC", "VIB_N1_LPT", "VIB_N2_HPT" })
            Assert.Equal("RECORDER_VIBRATION_NATIVE_UNVERIFIED", catalog.Find(code)!.Unit);
    }

    [Fact]
    public void Every_oem_indication_in_the_catalog_cites_its_source()
    {
        using var catalog = JsonDocument.Parse(SeedDocuments.Catalog);
        var indications = catalog.RootElement.GetProperty("parameters").EnumerateArray()
            .Select(parameter => parameter.GetProperty("oemIndication"))
            .Where(indication => indication.ValueKind == JsonValueKind.Object)
            .ToList();

        Assert.NotEmpty(indications);
        Assert.All(indications, indication => Assert.Equal("OEM-D3", indication.GetProperty("source").GetString()));
    }

    [Fact]
    public void The_profile_seed_is_valid_for_publishing_against_the_catalog_seed()
    {
        var validation = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true);

        Assert.Empty(validation.Issues);
    }

    [Fact]
    public void The_profile_covers_each_of_the_85_ordinals_exactly_once()
    {
        var profile = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true).Profile!;
        var ordinals = profile.Columns!.Select(column => column.Ordinal!.Value).Order().ToList();

        Assert.Equal(85, profile.ColumnCount);
        Assert.Equal(Enumerable.Range(0, 85), ordinals);
    }

    [Fact]
    public void The_two_utc_columns_are_bound_by_ordinal_and_occurrence()
    {
        var profile = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true).Profile!;
        var primary = profile.Columns!.Single(column => column.Ordinal == 1);
        var secondary = profile.Columns!.Single(column => column.Ordinal == 5);

        Assert.Equal(("UTC_PRIMARY", 1), (primary.ParameterCode, primary.HeaderOccurrence));
        Assert.Equal(("UTC_SECONDARY", 2), (secondary.ParameterCode, secondary.HeaderOccurrence));
        Assert.Equal(primary.ExactHeader!.Trim(), secondary.ExactHeader!.Trim());
    }

    [Fact]
    public void The_profile_headers_equal_the_real_recording_header_byte_for_byte()
    {
        var profile = DataProfileValidator.Validate(SeedDocuments.Profile, SeedDocuments.ValidatedCatalog, forPublish: true).Profile!;
        var recording = File.ReadAllBytes(RepositoryPaths.Fixtures("real", "2026-10-01_EP-DOA_24.csv"));
        var header = CsvRows.Read(recording, ',').First();

        foreach (var column in profile.Columns!)
            Assert.Equal(header[column.Ordinal!.Value], column.ExactHeader);
    }
}
