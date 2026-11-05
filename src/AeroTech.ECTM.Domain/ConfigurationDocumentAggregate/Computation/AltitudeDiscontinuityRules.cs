namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record AltitudeDiscontinuityRules(
        double? AltitudeFtPerSec,
        double? RadioHeightFtPerSec,
        double? UntrustedBeforeSec,
        double? UntrustedAfterSec);
}
