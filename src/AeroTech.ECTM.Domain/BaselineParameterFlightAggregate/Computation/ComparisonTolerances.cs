using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record ComparisonTolerances(long? RulesDocumentId, IReadOnlyList<BaselineConditionRule> Conditions)
    {
        public static readonly ComparisonTolerances None = new(null, []);

        public double? AircraftToleranceOf(string parameterCode)
            => Conditions.FirstOrDefault(rule => rule.PerEngine != true && rule.Parameter == parameterCode)?.Tolerance;

        public IEnumerable<BaselineConditionRule> EngineRules => Conditions.Where(rule => rule.PerEngine == true);

        public IEnumerable<string> AircraftCodes => Conditions.Where(rule => rule.PerEngine != true).Select(rule => rule.Parameter!);
    }
}
