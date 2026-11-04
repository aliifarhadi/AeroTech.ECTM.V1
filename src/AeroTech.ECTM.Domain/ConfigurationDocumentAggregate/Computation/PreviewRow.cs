namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record PreviewRow(int DataRowNumber, IReadOnlyList<PreviewCell> Cells);
}
