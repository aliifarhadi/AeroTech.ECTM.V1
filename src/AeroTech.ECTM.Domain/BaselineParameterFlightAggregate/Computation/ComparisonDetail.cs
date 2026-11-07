using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record ComparisonDetail(
        IReadOnlyList<ConditionCheck> Conditions,
        string? ObservedConfiguration,
        string? ReferenceConfiguration)
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public bool HasConditionOutsideTolerance => Conditions.Any(check => check.OutsideTolerance == true);

        public bool ConfigurationDiffers
            => ObservedConfiguration is not null
               && ReferenceConfiguration is not null
               && ObservedConfiguration != ReferenceConfiguration;

        public static ComparisonDetail Of(MeasurementContext reference, MeasurementContext observed, ComparisonTolerances tolerances)
        {
            var checks = new List<ConditionCheck>();

            foreach (var code in AircraftCodesOf(reference, tolerances))
                checks.Add(ConditionCheck.Of(code, false, observed.ConditionOf(code), reference.ConditionOf(code), tolerances.AircraftToleranceOf(code)));

            foreach (var rule in tolerances.EngineRules)
            {
                var code = rule.Parameter!;

                checks.Add(ConditionCheck.Of(code, true, observed.EngineValueOf(code), reference.EngineValueOf(code), rule.Tolerance));
            }

            return new ComparisonDetail(checks, observed.Configuration, reference.Configuration);
        }

        public static ComparisonDetail FromJson(string json) => JsonSerializer.Deserialize<ComparisonDetail>(json, Options)!;

        public string ToJson() => JsonSerializer.Serialize(this, Options);

        private static IReadOnlyList<string> AircraftCodesOf(MeasurementContext reference, ComparisonTolerances tolerances)
            => reference.Conditions
                .Select(condition => condition.ParameterCode)
                .Concat(tolerances.AircraftCodes)
                .Distinct(StringComparer.Ordinal)
                .ToList();
    }
}
