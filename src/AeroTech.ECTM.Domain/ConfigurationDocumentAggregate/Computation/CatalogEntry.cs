namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CatalogEntry(
        string Code,
        string? Unit,
        string? ValueKind,
        IReadOnlyList<int> Positions,
        bool IsDerived,
        PlausibilityRange? Plausibility = null,
        IReadOnlyList<string>? EnumValues = null)
    {
        public bool IsNumeric => ValueKind is "NUMERIC" or "INTEGER";
    }
}
