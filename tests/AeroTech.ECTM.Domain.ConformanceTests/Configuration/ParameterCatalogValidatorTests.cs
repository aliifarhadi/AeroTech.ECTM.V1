using System.Text.Json.Nodes;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class ParameterCatalogValidatorTests
{
    private const string EgtOverN2 = """
        {"code":"EGT_OVER_N2","kind":"RATIO","role":"ENGINE_PERFORMANCE","unitOut":"CELSIUS_PER_PERCENT","status":"CANDIDATE",
         "numerator":"EGT_C[pos]","denominator":"N2_PCT[pos]","guardZero":true}
        """;

    [Fact]
    public void Adding_EGT_OVER_N2_gives_a_per_position_derived_parameter()
    {
        var catalog = WithDerived(EgtOverN2);

        var validation = ParameterCatalogValidator.Validate(catalog);

        Assert.Empty(validation.Issues);
        Assert.Equal([1, 2], validation.Catalog!.Find("EGT_OVER_N2")!.Positions);
    }

    [Theory]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"TAT_X"}""", ParameterCatalogValidator.RefUnknown)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"X"}""", ParameterCatalogValidator.RefUnknown)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"egt"}""", ParameterCatalogValidator.RefInvalid)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"TAT_C[pos]","b":"TAT_C"}""", ParameterCatalogValidator.RefPositionInvalid)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C","b":"EGT_C[1]"}""", ParameterCatalogValidator.RefPositionInvalid)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"N1_PCT[1]"}""", ParameterCatalogValidator.UnitsDoNotReconcile)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"AIR_GROUND","b":"AIR_GROUND"}""", ParameterCatalogValidator.RefNotNumeric)]
    [InlineData("""{"code":"X","kind":"RATIO","role":"CONDITION","unitOut":"PERCENT","status":"CANDIDATE","numerator":"N1_PCT[1]","denominator":"N1_PCT[2]","guardZero":true}""", ParameterCatalogValidator.UnitsDoNotReconcile)]
    [InlineData("""{"code":"X","kind":"RATIO","role":"CONDITION","unitOut":"RATIO","status":"CANDIDATE","numerator":"N1_PCT[1]","denominator":"N1_PCT[2]"}""", ParameterCatalogValidator.FieldRequired)]
    [InlineData("""{"code":"X","kind":"POWER_RATIO","role":"CORRECTION","unitOut":"RATIO","status":"CANDIDATE","ref":"TAT_K","referenceConstant":288.15,"exponentConstantRef":"a"}""", ParameterCatalogValidator.ExponentRequiresCorrectionModel)]
    [InlineData("""{"code":"X","kind":"POWER_RATIO","role":"CORRECTION","unitOut":"RATIO","status":"CANDIDATE","ref":"TAT_K","referenceConstant":0,"exponentConstantRef":null}""", ParameterCatalogValidator.ConstantInvalid)]
    [InlineData("""{"code":"X","kind":"MEDIAN_SMOOTH","role":"CONDITION","unitOut":"FOOT_PER_MINUTE","status":"CANDIDATE","ref":"PRESSURE_ALT_FT","points":5}""", ParameterCatalogValidator.KindNotAvailable)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"EGT_C[2]","formula":"a-b"}""", ParameterCatalogValidator.FieldNotAllowed)]
    [InlineData("""{"code":"X","kind":"EXPRESSION","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE"}""", ParameterCatalogValidator.KindUnknown)]
    [InlineData("""{"code":"THETA","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"CANDIDATE","a":"EGT_C[1]","b":"EGT_C[2]"}""", ParameterCatalogValidator.CodeDuplicate)]
    [InlineData("""{"code":"X","kind":"DIFFERENCE","role":"CONDITION","unitOut":"CELSIUS","status":"APPROVED_BY_ME","a":"EGT_C[1]","b":"EGT_C[2]"}""", ParameterCatalogValidator.StatusUnknown)]
    public void An_invalid_derived_parameter_is_refused_with_its_reason(string definition, string expectedCode)
    {
        var validation = ParameterCatalogValidator.Validate(WithDerived(definition));

        Assert.Contains(expectedCode, validation.Issues.Select(issue => issue.Code));
    }

    [Fact]
    public void A_definition_cannot_reference_one_that_is_defined_after_it()
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            var derived = document["derivedParameters"]!.AsArray();
            var tatKelvin = derived[1]!;
            derived.RemoveAt(1);
            derived.Add(tatKelvin);
        });

        var validation = ParameterCatalogValidator.Validate(catalog);

        Assert.Contains(validation.Issues, issue => issue.Code == ParameterCatalogValidator.RefUnknown && issue.Message.Contains("TAT_K"));
    }

    [Theory]
    [InlineData("code", "egt_c", ParameterCatalogValidator.CodeInvalid)]
    [InlineData("role", "HEALTH", ParameterCatalogValidator.RoleUnknown)]
    [InlineData("unit", "DEGF", ParameterCatalogValidator.UnitUnknown)]
    [InlineData("valueKind", "FLOAT", ParameterCatalogValidator.ValueKindUnknown)]
    public void An_invalid_parameter_field_is_refused(string field, string value, string expectedCode)
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            ParameterNamed(document, "EGT_C")[field] = value;
        });

        Assert.Contains(expectedCode, ParameterCatalogValidator.Validate(catalog).Issues.Select(issue => issue.Code));
    }

    [Fact]
    public void A_position_other_than_one_or_two_is_refused()
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            ParameterNamed(document, "EGT_C")["positions"] = new JsonArray(1, 3);
        });

        Assert.Contains(ParameterCatalogValidator.PositionInvalid, ParameterCatalogValidator.Validate(catalog).Issues.Select(issue => issue.Code));
    }

    [Fact]
    public void A_plausibility_range_with_min_not_below_max_is_refused()
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            ParameterNamed(document, "N2_PCT")["plausibility"] = new JsonObject { ["min"] = 110, ["max"] = 0 };
        });

        Assert.Contains(ParameterCatalogValidator.PlausibilityInvalid, ParameterCatalogValidator.Validate(catalog).Issues.Select(issue => issue.Code));
    }

    [Fact]
    public void An_oem_indication_without_its_source_is_refused()
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            ParameterNamed(document, "N2_PCT")["oemIndication"] = new JsonObject { ["text"] = "idle ~60 %", ["source"] = null };
        });

        Assert.Contains(ParameterCatalogValidator.OemCitationRequired, ParameterCatalogValidator.Validate(catalog).Issues.Select(issue => issue.Code));
    }

    [Fact]
    public void A_parameter_defined_twice_is_refused()
    {
        var catalog = SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            var parameters = document["parameters"]!.AsArray();
            parameters.Add(ParameterNamed(document, "EGT_C").DeepClone());
        });

        Assert.Contains(ParameterCatalogValidator.CodeDuplicate, ParameterCatalogValidator.Validate(catalog).Issues.Select(issue => issue.Code));
    }

    private static string WithDerived(string definition)
        => SeedDocuments.Changed(SeedDocuments.Catalog, document =>
        {
            document["derivedParameters"]!.AsArray().Add(JsonNode.Parse(definition));
        });

    private static JsonObject ParameterNamed(JsonObject catalog, string code)
        => catalog["parameters"]!.AsArray().Single(parameter => parameter!["code"]!.GetValue<string>() == code)!.AsObject();
}
