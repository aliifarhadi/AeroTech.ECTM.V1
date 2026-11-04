using System.Text.Json;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class DerivedParameterEvaluatorTests
{
    private const double Tolerance = 1e-12;

    private const string EgtOverN2 = """
        {"code":"EGT_OVER_N2","kind":"RATIO","role":"ENGINE_PERFORMANCE","unitOut":"CELSIUS_PER_PERCENT","status":"CANDIDATE",
         "numerator":"EGT_C[pos]","denominator":"N2_PCT[pos]","guardZero":true}
        """;

    private const string WeightedN1 = """
        {"code":"WEIGHTED_N1","kind":"LINEAR_COMBINATION","role":"ENGINE_PERFORMANCE","unitOut":"PERCENT","status":"CANDIDATE",
         "terms":[{"ref":"N1_PCT[1]","coefficient":2},{"ref":"N1_PCT[2]","coefficient":3}],"offset":1}
        """;

    private const string HalfN1Difference = """
        {"code":"HALF_N1_DIFFERENCE","kind":"LINEAR_COMBINATION","role":"ENGINE_PERFORMANCE","unitOut":"PERCENT","status":"CANDIDATE",
         "terms":[{"ref":"N1_PCT[1]","coefficient":0.5},{"ref":"N1_PCT[2]","coefficient":-0.5}],"offset":0}
        """;

    private static readonly ValidatedCatalog Catalog = SeedDocuments.ValidatedCatalog;

    public static TheoryData<double?, double?, double?> EgtDifferenceVectors => new()
    {
        { 615, 600, 15 },
        { 600, 615, -15 },
        { 0, 0, 0 },
        { -10.5, 2.25, -12.75 },
        { 1000.25, 999.75, 0.5 },
        { 650.5, 650.5, 0 },
        { 1_000_000, 1, 999_999 },
        { 512.125, 0.125, 512 }
    };

    [Theory]
    [MemberData(nameof(EgtDifferenceVectors))]
    public void EGT_DIFF_E2_E1_C_subtracts_engine_one_from_engine_two(double? egtTwo, double? egtOne, double? expected)
    {
        var evaluation = Evaluate(SeedDefinition("EGT_DIFF_E2_E1_C"), null, ("EGT_C[2]", egtTwo, "CELSIUS"), ("EGT_C[1]", egtOne, "CELSIUS"));

        Assert.Equal(DerivedParameterEvaluator.Available, evaluation.Status);
        Assert.Equal(expected!.Value, evaluation.Value!.Value, Tolerance);
        Assert.Equal("CELSIUS", evaluation.Unit);
    }

    [Fact]
    public void A_null_engine_reading_gives_no_difference()
    {
        var evaluation = Evaluate(SeedDefinition("EGT_DIFF_E2_E1_C"), null, ("EGT_C[2]", null, "CELSIUS"), ("EGT_C[1]", 600, "CELSIUS"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.NullInput);
    }

    [Fact]
    public void A_missing_engine_reading_gives_no_difference()
    {
        var evaluation = Evaluate(SeedDefinition("EGT_DIFF_E2_E1_C"), null, ("EGT_C[2]", 615, "CELSIUS"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.MissingInput);
    }

    [Fact]
    public void A_reading_in_the_wrong_unit_gives_no_difference()
    {
        var evaluation = Evaluate(SeedDefinition("EGT_DIFF_E2_E1_C"), null, ("EGT_C[2]", 888.15, "KELVIN"), ("EGT_C[1]", 600, "CELSIUS"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.UnitMismatch);
    }

    public static TheoryData<int, double, double, double> RatioVectors => new()
    {
        { 1, 600, 96, 6.25 },
        { 2, 630, 105, 6 },
        { 1, 0, 50, 0 },
        { 2, -12, 4, -3 },
        { 1, 7.5, 2.5, 3 },
        { 2, 1, 3, 1.0 / 3.0 },
        { 1, 650, 100, 6.5 },
        { 2, 0.75, 0.25, 3 }
    };

    [Theory]
    [MemberData(nameof(RatioVectors))]
    public void EGT_OVER_N2_divides_on_the_requested_position(int position, double egt, double n2, double expected)
    {
        var evaluation = Evaluate(Definition(EgtOverN2), position, ($"EGT_C[{position}]", egt, "CELSIUS"), ($"N2_PCT[{position}]", n2, "PERCENT"));

        Assert.Equal(DerivedParameterEvaluator.Available, evaluation.Status);
        Assert.Equal(expected, evaluation.Value!.Value, Tolerance);
        Assert.Equal($"EGT_OVER_N2[{position}]", evaluation.Code);
    }

    [Fact]
    public void A_zero_denominator_gives_no_ratio()
    {
        var evaluation = Evaluate(Definition(EgtOverN2), 1, ("EGT_C[1]", 600, "CELSIUS"), ("N2_PCT[1]", 0, "PERCENT"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.DenominatorZero);
    }

    [Fact]
    public void A_per_position_definition_needs_a_position()
    {
        var evaluation = Evaluate(Definition(EgtOverN2), null, ("EGT_C[1]", 600, "CELSIUS"), ("N2_PCT[1]", 96, "PERCENT"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.PositionRequired);
    }

    [Fact]
    public void Inputs_of_the_other_position_are_not_used()
    {
        var evaluation = Evaluate(Definition(EgtOverN2), 2, ("EGT_C[1]", 600, "CELSIUS"), ("N2_PCT[1]", 96, "PERCENT"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.MissingInput);
    }

    [Fact]
    public void An_overflowing_ratio_is_not_a_number()
    {
        var evaluation = Evaluate(Definition(EgtOverN2), 1, ("EGT_C[1]", 1e308, "CELSIUS"), ("N2_PCT[1]", 1e-308, "PERCENT"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.NonFinite);
    }

    public static TheoryData<double, double> TatKelvinVectors => new()
    {
        { 15, 288.15 },
        { 0, 273.15 },
        { -56.5, 216.65 },
        { -273.15, 0 },
        { 40, 313.15 },
        { 100, 373.15 },
        { -40, 233.15 }
    };

    [Theory]
    [MemberData(nameof(TatKelvinVectors))]
    public void TAT_K_adds_273_15_to_the_celsius_reading(double tatCelsius, double expectedKelvin)
    {
        var evaluation = Evaluate(SeedDefinition("TAT_K"), null, ("TAT_C", tatCelsius, "CELSIUS"));

        Assert.Equal(DerivedParameterEvaluator.Available, evaluation.Status);
        Assert.Equal(expectedKelvin, evaluation.Value!.Value, 1e-9);
        Assert.Equal("KELVIN", evaluation.Unit);
    }

    [Fact]
    public void A_linear_combination_weights_every_term_and_adds_the_offset()
    {
        var weighted = Evaluate(Definition(WeightedN1), null, ("N1_PCT[1]", 10, "PERCENT"), ("N1_PCT[2]", 20, "PERCENT"));
        var halfDifference = Evaluate(Definition(HalfN1Difference), null, ("N1_PCT[1]", 10, "PERCENT"), ("N1_PCT[2]", 20, "PERCENT"));

        Assert.Equal(81, weighted.Value);
        Assert.Equal(-5, halfDifference.Value);
    }

    [Fact]
    public void A_linear_combination_with_a_missing_term_gives_no_value()
    {
        var evaluation = Evaluate(Definition(WeightedN1), null, ("N1_PCT[1]", 10, "PERCENT"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.MissingInput);
    }

    [Fact]
    public void A_linear_combination_with_a_null_term_gives_no_value()
    {
        var evaluation = Evaluate(SeedDefinition("TAT_K"), null, ("TAT_C", null, "CELSIUS"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.NullInput);
    }

    [Fact]
    public void A_linear_combination_in_the_wrong_unit_gives_no_value()
    {
        var evaluation = Evaluate(SeedDefinition("TAT_K"), null, ("TAT_C", 288.15, "KELVIN"));

        AssertUnavailable(evaluation, DerivedParameterEvaluator.UnitMismatch);
    }

    public static TheoryData<string, string, double, string, double> PowerRatioVectors => new()
    {
        { "THETA", "TAT_K", 288.15, "KELVIN", 1 },
        { "THETA", "TAT_K", 0, "KELVIN", 0 },
        { "THETA", "TAT_K", 576.3, "KELVIN", 2 },
        { "THETA", "TAT_K", 144.075, "KELVIN", 0.5 },
        { "DELTA", "STATIC_PRESSURE_INHG", 29.92126, "INHG", 1 },
        { "DELTA", "STATIC_PRESSURE_INHG", 14.96063, "INHG", 0.5 },
        { "DELTA", "STATIC_PRESSURE_INHG", 0, "INHG", 0 },
        { "DELTA", "STATIC_PRESSURE_INHG", 59.84252, "INHG", 2 }
    };

    [Theory]
    [MemberData(nameof(PowerRatioVectors))]
    public void A_power_ratio_divides_by_its_reference_constant(string code, string input, double value, string unit, double expected)
    {
        var evaluation = Evaluate(SeedDefinition(code), null, (input, value, unit));

        Assert.Equal(DerivedParameterEvaluator.Available, evaluation.Status);
        Assert.Equal(expected, evaluation.Value!.Value, Tolerance);
        Assert.Equal("RATIO", evaluation.Unit);
    }

    [Fact]
    public void A_power_ratio_with_a_null_input_gives_no_value()
    {
        AssertUnavailable(Evaluate(SeedDefinition("THETA"), null, ("TAT_K", null, "KELVIN")), DerivedParameterEvaluator.NullInput);
    }

    [Fact]
    public void A_power_ratio_with_a_missing_input_gives_no_value()
    {
        AssertUnavailable(Evaluate(SeedDefinition("DELTA"), null), DerivedParameterEvaluator.MissingInput);
    }

    [Fact]
    public void A_power_ratio_in_the_wrong_unit_gives_no_value()
    {
        AssertUnavailable(Evaluate(SeedDefinition("THETA"), null, ("TAT_K", 15, "CELSIUS")), DerivedParameterEvaluator.UnitMismatch);
    }

    [Fact]
    public void The_explanation_shows_the_formula_with_the_substituted_numbers_and_constants()
    {
        var evaluation = Evaluate(SeedDefinition("THETA"), null, ("TAT_K", 576.3, "KELVIN"));

        Assert.Equal("TAT_K / 288.15 = 576.3 / 288.15 = 2", evaluation.Formula);
        Assert.Equal(288.15, evaluation.Constants["referenceConstant"]);
        Assert.Equal(new EvaluatedInput("TAT_K", 576.3, "KELVIN"), Assert.Single(evaluation.Inputs));
    }

    [Fact]
    public void The_same_inputs_give_the_same_bits()
    {
        var first = Evaluate(Definition(EgtOverN2), 2, ("EGT_C[2]", 1, "CELSIUS"), ("N2_PCT[2]", 3, "PERCENT"));
        var second = Evaluate(Definition(EgtOverN2), 2, ("EGT_C[2]", 1, "CELSIUS"), ("N2_PCT[2]", 3, "PERCENT"));

        Assert.Equal(BitConverter.DoubleToInt64Bits(first.Value!.Value), BitConverter.DoubleToInt64Bits(second.Value!.Value));
    }

    private static TypedEvaluation Evaluate(DerivedParameterDefinition definition, int? position, params (string Ref, double? Value, string Unit)[] inputs)
    {
        var typedInputs = inputs.ToDictionary(input => input.Ref, input => new TypedInput(input.Value, input.Unit), StringComparer.Ordinal);

        return DerivedParameterEvaluator.Evaluate(definition, Catalog.Entries, position, typedInputs);
    }

    private static void AssertUnavailable(TypedEvaluation evaluation, string expectedStatus)
    {
        Assert.Equal(expectedStatus, evaluation.Status);
        Assert.Null(evaluation.Value);
        Assert.NotEmpty(evaluation.Reasons);
        Assert.Null(evaluation.Formula);
    }

    private static DerivedParameterDefinition SeedDefinition(string code) => Catalog.DerivedParameters.Single(definition => definition.Code == code);

    private static DerivedParameterDefinition Definition(string json)
    {
        using var document = JsonDocument.Parse(json);
        var validation = ParameterCatalogValidator.ValidateDefinition(document.RootElement, Catalog.Entries, "$", allowExistingCode: false);

        Assert.Empty(validation.Issues);

        return validation.Definition!;
    }
}
