namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CatalogParameter(
        string? Code,
        string? Role,
        string? Unit,
        string? ValueKind,
        IReadOnlyList<int>? Positions,
        PlausibilityRange? Plausibility,
        string? Note,
        OemIndication? OemIndication,
        IReadOnlyList<string>? EnumValues = null);
}
