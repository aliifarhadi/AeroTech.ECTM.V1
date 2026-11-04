namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record CatalogValidation(ValidatedCatalog? Catalog, IReadOnlyList<ValidationIssue> Issues)
    {
        public bool IsValid => Issues.Count == 0;
    }
}
