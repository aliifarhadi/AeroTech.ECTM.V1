namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ProfilePreview(
        bool SignatureMatches,
        int ExpectedColumnCount,
        int HeaderColumnCount,
        IReadOnlyList<HeaderMismatch> HeaderMismatches,
        int DataRowCount,
        int RaggedRowCount,
        IReadOnlyList<PreviewRow> FirstRows,
        IReadOnlyList<PreviewRow> LastRows,
        IReadOnlyList<ColumnParseSummary> ParseSummary,
        IReadOnlyList<EnginePositionChannels> Engines);
}
