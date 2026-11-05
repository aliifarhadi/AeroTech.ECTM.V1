using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightReviewAggregate.Arguments
{
    public sealed record ReviewEntryArgs(ReviewAction Action, string? Actor, ExclusionReason? ReasonCode, string? Note, FlightState FlightStateAfter);
}
