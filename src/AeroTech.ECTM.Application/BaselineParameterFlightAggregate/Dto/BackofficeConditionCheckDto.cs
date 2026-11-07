using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto
{
    public sealed record BackofficeConditionCheckDto(
        string ParameterCode,
        bool PerEngine,
        double? Observed,
        double? Reference,
        double? Difference,
        double? Tolerance,
        bool? OutsideTolerance)
    {
        public static BackofficeConditionCheckDto From(ConditionCheck check)
            => new(check.ParameterCode, check.PerEngine, check.Observed, check.Reference, check.Difference, check.Tolerance, check.OutsideTolerance);
    }
}
