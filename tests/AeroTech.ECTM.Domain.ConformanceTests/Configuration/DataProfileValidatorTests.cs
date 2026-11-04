using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class DataProfileValidatorTests
{
    [Fact]
    public void Swapped_occurrences_of_the_duplicate_utc_header_are_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 1)["headerOccurrence"] = 2;
            SeedDocuments.ColumnAt(document, 5)["headerOccurrence"] = 1;
        });

        var codes = CodesOf(profile, forPublish: false);

        Assert.Equal(2, codes.Count(code => code == DataProfileValidator.OccurrenceMismatch));
    }

    [Fact]
    public void Two_columns_on_one_parameter_and_position_collide()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 12)["position"] = 1;
        });

        var validation = DataProfileValidator.Validate(profile, SeedDocuments.ValidatedCatalog, forPublish: false);
        var collision = Assert.Single(validation.Issues, issue => issue.Code == DataProfileValidator.ColumnCollision);

        Assert.Contains("N1_PCT[1] is already mapped by ordinal 11", collision.Message);
    }

    [Fact]
    public void An_unknown_source_unit_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 15)["sourceUnit"] = "FAHRENHEIT";
        });

        Assert.Contains(DataProfileValidator.UnitUnknown, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void A_known_unit_that_differs_from_the_catalog_without_conversion_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 15)["sourceUnit"] = "KELVIN";
        });

        Assert.Contains(DataProfileValidator.UnitMismatch, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void A_conversion_other_than_identity_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 45)["conversion"] = "INHG_TO_KPA";
        });

        Assert.Contains(DataProfileValidator.ConversionUnsupported, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void A_duplicated_ordinal_and_the_ordinal_it_hides_are_both_reported()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 64)["ordinal"] = 63;
        });

        var codes = CodesOf(profile, forPublish: false);

        Assert.Contains(DataProfileValidator.OrdinalDuplicate, codes);
        Assert.Contains(DataProfileValidator.OrdinalMissing, codes);
    }

    [Fact]
    public void A_missing_column_does_not_match_the_column_count()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            document["columns"]!.AsArray().RemoveAt(84);
        });

        Assert.Contains(DataProfileValidator.ColumnCountMismatch, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void Publishing_without_a_required_engine_channel_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 16)["parameterCode"] = null;
            SeedDocuments.ColumnAt(document, 16)["position"] = null;
        });

        var draftCodes = CodesOf(profile, forPublish: false);
        var publishValidation = DataProfileValidator.Validate(profile, SeedDocuments.ValidatedCatalog, forPublish: true);

        Assert.DoesNotContain(DataProfileValidator.RequiredChannelMissing, draftCodes);
        var missing = Assert.Single(publishValidation.Issues);
        Assert.Contains("EGT_C[2]", missing.Message);
    }

    [Fact]
    public void A_parameter_missing_from_the_catalog_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 15)["parameterCode"] = "EGT_X";
        });

        Assert.Contains(DataProfileValidator.ParameterUnknown, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void A_derived_parameter_cannot_be_a_recorder_column()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 83)["parameterCode"] = "EGT_DIFF_E2_E1_C";
        });

        Assert.Contains(DataProfileValidator.ParameterIsDerived, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void A_position_that_the_parameter_does_not_have_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            SeedDocuments.ColumnAt(document, 3)["position"] = 1;
        });

        Assert.Contains(DataProfileValidator.PositionInvalid, CodesOf(profile, forPublish: false));
    }

    [Fact]
    public void Without_an_active_catalog_the_profile_cannot_be_checked()
    {
        var validation = DataProfileValidator.Validate(SeedDocuments.Profile, catalog: null, forPublish: false);

        Assert.Equal(DataProfileValidator.ActiveCatalogMissing, Assert.Single(validation.Issues).Code);
    }

    [Fact]
    public void A_field_outside_the_schema_is_refused()
    {
        var profile = SeedDocuments.Changed(SeedDocuments.Profile, document =>
        {
            document["formula"] = "anything";
        });

        Assert.Equal(ContentJson.ShapeInvalid, Assert.Single(CodesOf(profile, forPublish: false)));
    }

    private static List<string> CodesOf(string profile, bool forPublish)
        => DataProfileValidator.Validate(profile, SeedDocuments.ValidatedCatalog, forPublish).Issues.Select(issue => issue.Code).ToList();
}
