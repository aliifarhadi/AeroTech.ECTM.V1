using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record CruiseSnapshot(
        string Selector,
        int Position,
        SnapshotAvailability Availability,
        SnapshotReason? Reason,
        int? BlockStartIndex,
        int? BlockEndIndexInclusive,
        double? BlockSeconds,
        int? ReportStartIndex,
        int? ReportEndIndexInclusive,
        IReadOnlyList<ParameterSummary> Values,
        SnapshotContext? Context,
        CruiseGateTrace Gates)
    {
        public bool HasBlock => BlockStartIndex is not null;
    }
}
