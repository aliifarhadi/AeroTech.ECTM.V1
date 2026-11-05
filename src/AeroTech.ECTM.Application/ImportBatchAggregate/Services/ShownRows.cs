using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed record ShownRows(IReadOnlyList<RecordingRow> First, IReadOnlyList<RecordingRow> Last);
}
