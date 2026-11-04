namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DataProfileContent(
        string? ProfileCode,
        string? FileType,
        string? Delimiter,
        string? Encoding,
        int? ColumnCount,
        string? TimeParameterCode,
        string? Source,
        IReadOnlyList<DataProfileColumn>? Columns);
}
