using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class TakeoffEgtPeakFinder
    {
        public static TakeoffEgtPeakObservation Find(SnapshotFrame frame, int position, TakeoffSearch? search, TakeoffN1PeakContext n1Peak)
        {
            if (search is null)
                return TakeoffEgtPeakObservation.NotEvaluable(position, SnapshotReason.NO_LIFTOFF_ANCHOR_AND_NO_HIGH_N1);

            var egt = frame.Numbers(SnapshotFrame.KeyOf(frame.Rules.Channels!.EngineEgt!, position));

            if (TakeoffN1PeakFinder.FirstMaximum(egt, search.SamplesInBlockOf(frame, search.StartIndex)) is not { } peak)
                return TakeoffEgtPeakObservation.NotEvaluable(position, SnapshotReason.NO_EGT_IN_SEARCH_WINDOW, search);

            var valuesAtPeak = frame.Rules.EngineParameters!
                .Select(code => frame.SummaryOf(SnapshotFrame.KeyOf(code, position), [peak]))
                .ToList();
            var offset = n1Peak.PeakElapsed is { } n1PeakElapsed ? frame.Elapsed[peak] - n1PeakElapsed : (double?)null;

            return new TakeoffEgtPeakObservation(
                position,
                SnapshotAvailability.AVAILABLE,
                null,
                search.Basis,
                peak,
                frame.Elapsed[peak],
                egt[peak],
                offset,
                valuesAtPeak);
        }
    }
}
