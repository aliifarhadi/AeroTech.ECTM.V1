using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record PairedSnapshot(
        string Selector,
        string? MinuendKey,
        string? SubtrahendKey,
        SnapshotAvailability Availability,
        SnapshotReason? Reason,
        int? JointStartIndex,
        int? JointEndIndexInclusive,
        int? ReportStartIndex,
        int? ReportEndIndexInclusive,
        int MatchedSamples,
        double? SumDifference,
        double? MeanDifference);
}
