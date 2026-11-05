using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Computation
{
    public sealed record ImportItemTally(long ImportBatchId, ImportItemOutcome Outcome, ImportItemState State, int Count);
}
