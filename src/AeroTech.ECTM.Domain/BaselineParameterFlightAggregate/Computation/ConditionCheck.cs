namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record ConditionCheck(
        string ParameterCode,
        bool PerEngine,
        double? Observed,
        double? Reference,
        double? Difference,
        double? Tolerance,
        bool? OutsideTolerance)
    {
        public static ConditionCheck Of(string parameterCode, bool perEngine, double? observed, double? reference, double? tolerance)
        {
            double? difference = null;
            bool? outsideTolerance = null;

            if (observed is { } observedValue && reference is { } referenceValue)
                difference = observedValue - referenceValue;

            if (difference is { } knownDifference && tolerance is { } limit)
                outsideTolerance = Math.Abs(knownDifference) > limit;

            return new ConditionCheck(parameterCode, perEngine, observed, reference, difference, tolerance, outsideTolerance);
        }
    }
}
