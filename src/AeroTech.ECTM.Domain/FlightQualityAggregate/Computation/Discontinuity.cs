namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed record Discontinuity(
        int Index,
        double? DAltFtPerSec,
        double? DRhFtPerSec,
        double? RadioHeightBeforeFt,
        string Code);
}
