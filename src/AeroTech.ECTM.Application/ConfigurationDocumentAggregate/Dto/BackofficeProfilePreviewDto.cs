using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeProfilePreviewDto(
        long DocumentId,
        string FileName,
        string FileSha256,
        bool SignatureMatches,
        int ExpectedColumnCount,
        int HeaderColumnCount,
        IReadOnlyList<HeaderMismatch> HeaderMismatches,
        int DataRowCount,
        int RaggedRowCount,
        IReadOnlyList<PreviewRow> FirstRows,
        IReadOnlyList<PreviewRow> LastRows,
        IReadOnlyList<ColumnParseSummary> ParseSummary,
        IReadOnlyList<EnginePositionChannels> Engines)
    {
        public static BackofficeProfilePreviewDto From(long documentId, string fileName, string fileSha256, ProfilePreview preview)
            => new(
                documentId,
                fileName,
                fileSha256,
                preview.SignatureMatches,
                preview.ExpectedColumnCount,
                preview.HeaderColumnCount,
                preview.HeaderMismatches,
                preview.DataRowCount,
                preview.RaggedRowCount,
                preview.FirstRows,
                preview.LastRows,
                preview.ParseSummary,
                preview.Engines);
    }
}
