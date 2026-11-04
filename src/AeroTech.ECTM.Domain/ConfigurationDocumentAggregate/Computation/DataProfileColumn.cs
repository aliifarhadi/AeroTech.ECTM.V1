namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DataProfileColumn(
        int? Ordinal,
        string? ExactHeader,
        int? HeaderOccurrence,
        string? ParameterCode,
        int? Position,
        string? SourceUnit,
        string? Conversion,
        IReadOnlyList<string>? MissingTokens);
}
