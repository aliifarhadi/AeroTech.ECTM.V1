namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DefinitionValidation(
        DerivedParameterDefinition? Definition,
        CatalogEntry? Entry,
        IReadOnlyList<ValidationIssue> Issues)
    {
        public bool IsValid => Issues.Count == 0;
    }
}
