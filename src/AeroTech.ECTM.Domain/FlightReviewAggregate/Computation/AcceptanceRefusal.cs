using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightReviewAggregate.Computation
{
    public sealed record AcceptanceRefusal(string Code, SnapshotKind? Kind = null, int? Position = null);
}
