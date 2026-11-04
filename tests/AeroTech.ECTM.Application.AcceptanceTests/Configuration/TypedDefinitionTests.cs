using System.Net;
using System.Text.Json.Nodes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Configuration;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class TypedDefinitionTests(MigratedDatabaseFixture fixture)
{
    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task The_closed_list_of_kinds_says_which_ones_evaluate_now()
    {
        using var response = await _configuration.GetAsync("config/typed-definitions/kinds");
        var kinds = (await ApiResponses.DataOf(response)).EnumerateArray().ToList();

        var evaluable = kinds.Where(kind => kind.GetProperty("evaluable").GetBoolean()).Select(kind => kind.GetProperty("code").GetString()).ToList();
        var median = kinds.Single(kind => kind.GetProperty("code").GetString() == "MEDIAN_SMOOTH");

        Assert.Equal(10, kinds.Count);
        Assert.Equal(["DIFFERENCE", "RATIO", "LINEAR_COMBINATION", "POWER_RATIO"], evaluable);
        Assert.StartsWith("P05", median.GetProperty("availableFrom").GetString());
    }

    [Fact]
    public async Task A_seed_definition_evaluates_against_the_active_catalog()
    {
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        var definition = ConfigurationApi.SeedOf(ConfigurationApi.Catalog)["derivedParameters"]!.AsArray()
            .Single(item => item!["code"]!.GetValue<string>() == "EGT_DIFF_E2_E1_C");

        using var response = await _configuration.PostAsync("config/typed-definitions/evaluate", new
        {
            definition,
            inputs = new Dictionary<string, object>
            {
                ["EGT_C[2]"] = new { value = 615, unit = "CELSIUS" },
                ["EGT_C[1]"] = new { value = 600, unit = "CELSIUS" }
            }
        });

        var result = await ApiResponses.DataOf(response);
        Assert.Equal(15, result.GetProperty("value").GetDouble());
        Assert.Equal("CELSIUS", result.GetProperty("unit").GetString());
    }

    [Fact]
    public async Task A_kind_of_a_later_phase_is_refused_with_its_reason()
    {
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);

        using var response = await _configuration.PostAsync("config/typed-definitions/evaluate", new
        {
            definition = JsonNode.Parse("""{"code":"ALT_RATE_SMOOTH_FPM","kind":"MEDIAN_SMOOTH","role":"CONDITION","unitOut":"FOOT_PER_MINUTE","status":"CANDIDATE","ref":"PRESSURE_ALT_FT","points":5}"""),
            inputs = new Dictionary<string, object>()
        });

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80115, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_missing_reading_gives_a_status_and_no_number()
    {
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);
        var definition = ConfigurationApi.SeedOf(ConfigurationApi.Catalog)["derivedParameters"]!.AsArray()
            .Single(item => item!["code"]!.GetValue<string>() == "THETA");

        using var response = await _configuration.PostAsync("config/typed-definitions/evaluate", new { definition, inputs = new Dictionary<string, object>() });

        var result = await ApiResponses.DataOf(response);
        Assert.Equal("MISSING_INPUT", result.GetProperty("status").GetString());
        Assert.Equal(System.Text.Json.JsonValueKind.Null, result.GetProperty("value").ValueKind);
    }
}
