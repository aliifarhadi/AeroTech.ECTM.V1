namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record ParameterStatistics(
        string ParameterKey,
        int Samples,
        int Valued,
        int Missing,
        int GapStarts,
        int Masked,
        double? Min,
        double? Max,
        double? Mean,
        double? Median,
        double? Percentile,
        double? StandardDeviation);
}
