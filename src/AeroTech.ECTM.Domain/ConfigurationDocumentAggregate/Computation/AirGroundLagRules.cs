namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record AirGroundLagRules(
        double? RadioHeightAboveFt,
        double? MinSustainedSec,
        double? AirborneRadioHeightFt);
}
