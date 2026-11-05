using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Arguments
{
    public sealed record ImportItemArgs(
        string FileName,
        string? ContainerName,
        long SizeBytes,
        string? Sha256,
        long? SourceFileId,
        ImportItemOutcome Outcome,
        string? RejectionCode,
        long? ExistingFlightId,
        ImportItemPreview Preview);
}
