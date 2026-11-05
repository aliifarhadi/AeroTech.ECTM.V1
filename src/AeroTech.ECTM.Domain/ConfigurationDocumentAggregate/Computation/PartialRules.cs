namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record PartialRules(
        double? StartRadioHeightAboveFt,
        double? EndGroundspeedAtLeastKt);
}
