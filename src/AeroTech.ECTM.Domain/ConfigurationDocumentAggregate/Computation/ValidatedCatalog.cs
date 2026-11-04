namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ValidatedCatalog(
        IReadOnlyDictionary<string, CatalogEntry> Entries,
        IReadOnlyList<DerivedParameterDefinition> DerivedParameters)
    {
        public CatalogEntry? Find(string code) => Entries.GetValueOrDefault(code);
    }
}
