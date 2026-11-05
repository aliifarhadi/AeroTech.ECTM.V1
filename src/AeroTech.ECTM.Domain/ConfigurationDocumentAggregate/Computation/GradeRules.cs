namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record GradeRules(
        double? AMinCoveragePct,
        IReadOnlyList<AllowedMask>? MasksAllowedForA);
}
