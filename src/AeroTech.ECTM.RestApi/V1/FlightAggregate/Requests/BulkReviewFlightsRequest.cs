using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests
{
    public sealed record BulkReviewFlightsRequest(IReadOnlyList<long> FlightIds, ReviewAction Action, ExclusionReason? ReasonCode, string? Note);
}
