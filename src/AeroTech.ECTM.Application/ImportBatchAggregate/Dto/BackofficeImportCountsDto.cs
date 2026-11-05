using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Dto
{
    public sealed record BackofficeImportCountsDto(
        int Items,
        int New,
        int Duplicate,
        int Rejected,
        int Previewed,
        int Committed,
        int Skipped,
        int Failed)
    {
        public static BackofficeImportCountsDto From(IReadOnlyList<ImportItemTally> tallies)
            => new(
                tallies.Sum(tally => tally.Count),
                CountOf(tallies, ImportItemOutcome.NEW),
                CountOf(tallies, ImportItemOutcome.DUPLICATE),
                CountOf(tallies, ImportItemOutcome.REJECTED),
                CountOf(tallies, ImportItemState.PREVIEWED),
                CountOf(tallies, ImportItemState.COMMITTED),
                CountOf(tallies, ImportItemState.SKIPPED),
                CountOf(tallies, ImportItemState.FAILED));

        private static int CountOf(IReadOnlyList<ImportItemTally> tallies, ImportItemOutcome outcome)
            => tallies.Where(tally => tally.Outcome == outcome).Sum(tally => tally.Count);

        private static int CountOf(IReadOnlyList<ImportItemTally> tallies, ImportItemState state)
            => tallies.Where(tally => tally.State == state).Sum(tally => tally.Count);
    }
}
