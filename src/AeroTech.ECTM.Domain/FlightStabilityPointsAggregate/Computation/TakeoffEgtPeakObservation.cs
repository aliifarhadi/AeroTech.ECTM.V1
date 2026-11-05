using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record TakeoffEgtPeakObservation(
        int Position,
        SnapshotAvailability Availability,
        SnapshotReason? Reason,
        TakeoffSearchBasis? SearchBasis,
        int? PeakIndex,
        double? PeakElapsed,
        double? EgtPeak,
        double? OffsetFromN1PeakSec,
        IReadOnlyList<ParameterSummary> ValuesAtPeak)
    {
        public static TakeoffEgtPeakObservation NotEvaluable(int position, SnapshotReason reason, TakeoffSearch? search = null)
            => new(position, SnapshotAvailability.NOT_EVALUABLE, reason, search?.Basis, null, null, null, null, []);
    }
}
