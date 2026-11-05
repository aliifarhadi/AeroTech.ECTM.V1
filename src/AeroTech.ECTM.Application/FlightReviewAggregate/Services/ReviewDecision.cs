using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Services
{
    public sealed record ReviewDecision(ReviewAction Action, ExclusionReason? ReasonCode, string? Note, long? AnalysisRevisionId);
}
