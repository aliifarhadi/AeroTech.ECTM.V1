using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record TakeoffN1PeakContext(
        int Position,
        SnapshotAvailability Availability,
        SnapshotReason? Reason,
        TakeoffSearchBasis? SearchBasis,
        int? SearchStartIndex,
        int? SearchEndIndexInclusive,
        int? PeakIndex,
        double? PeakElapsed,
        int? StartIndex,
        int? EndIndexInclusive,
        int? ValidCount,
        int? ValidEgtCount,
        double? N1Peak,
        double? EgtAtPeak,
        IReadOnlyList<ParameterSummary> Values,
        SnapshotContext? Context,
        SnapshotContextStatus? ContextStatus)
    {
        public static TakeoffN1PeakContext NotEvaluable(int position, SnapshotReason reason, TakeoffSearch? search = null, int? peakIndex = null)
            => new(position, SnapshotAvailability.NOT_EVALUABLE, reason, search?.Basis, search?.StartIndex, search?.EndIndexInclusive,
                peakIndex, null, null, null, null, null, null, null, [], null, null);
    }
}
