namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ColumnParseSummary(int Ordinal, string ParameterKey, int MissingCount, int UnparseableCount);
}
