using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record RecordingRow(int SampleIndex, IReadOnlyList<PreviewCell> Cells);
}
