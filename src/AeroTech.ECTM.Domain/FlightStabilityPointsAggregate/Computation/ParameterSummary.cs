namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ParameterSummary(string ParameterKey, int Count, double? Mean, double? Min, double? Max)
    {
        public static ParameterSummary Of(string parameterKey, IEnumerable<double?> values)
        {
            var present = values.OfType<double>().ToList();

            if (present.Count == 0)
                return new ParameterSummary(parameterKey, 0, null, null, null);

            return new ParameterSummary(parameterKey, present.Count, CompensatedSum.Of(present) / present.Count, present.Min(), present.Max());
        }
    }
}
