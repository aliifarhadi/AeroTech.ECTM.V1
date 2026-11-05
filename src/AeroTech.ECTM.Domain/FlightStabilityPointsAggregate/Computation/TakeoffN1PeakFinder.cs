using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class TakeoffN1PeakFinder
    {
        public static TakeoffN1PeakContext Find(SnapshotFrame frame, int position, TakeoffSearch? search)
        {
            if (search is null)
                return TakeoffN1PeakContext.NotEvaluable(position, SnapshotReason.NO_LIFTOFF_ANCHOR_AND_NO_HIGH_N1);

            var channels = frame.Rules.Channels!;
            var takeoff = frame.Rules.Takeoff!;
            var n1 = frame.Numbers(SnapshotFrame.KeyOf(channels.EngineN1!, position));
            var egt = frame.Numbers(SnapshotFrame.KeyOf(channels.EngineEgt!, position));

            if (FirstMaximum(n1, search.SamplesInBlockOf(frame, search.StartIndex)) is not { } peak)
                return TakeoffN1PeakContext.NotEvaluable(position, SnapshotReason.NO_N1, search);

            var start = frame.FirstAtOrAfter(frame.Elapsed[peak] - takeoff.WindowBeforePeakSec!.Value);
            var end = frame.LastAtOrBefore(frame.Elapsed[peak] + takeoff.WindowAfterPeakSec!.Value);
            var valid = frame.Range(start, end)
                .Where(index => frame.SameBlock(index, peak) && n1[index] is not null)
                .ToList();

            if (valid.Count < takeoff.MinValidN1Samples)
                return TakeoffN1PeakContext.NotEvaluable(position, SnapshotReason.INSUFFICIENT_WINDOW, search, peak);

            var validEgtCount = valid.Count(index => egt[index] is not null);

            if (validEgtCount == 0)
                return TakeoffN1PeakContext.NotEvaluable(position, SnapshotReason.NO_EGT_IN_SELECTED_WINDOW, search, peak);

            if (!search.SamplesInBlockOf(frame, peak).Any(index => egt[index] is not null))
                return TakeoffN1PeakContext.NotEvaluable(position, SnapshotReason.NO_EGT_IN_SEARCH_WINDOW, search, peak);

            var values = frame.Rules.EngineParameters!
                .Select(code => frame.SummaryOf(SnapshotFrame.KeyOf(code, position), valid))
                .ToList();
            var contextVerified = !valid.Any(index => frame.IsCoherenceSuspect(index) || frame.IsUntrusted(index));

            return new TakeoffN1PeakContext(
                position,
                SnapshotAvailability.AVAILABLE,
                null,
                search.Basis,
                search.StartIndex,
                search.EndIndexInclusive,
                peak,
                frame.Elapsed[peak],
                start,
                end,
                valid.Count,
                validEgtCount,
                n1[peak],
                egt[peak],
                values,
                SnapshotContext.Of(frame, valid),
                contextVerified ? SnapshotContextStatus.VERIFIED : SnapshotContextStatus.CONTEXT_UNVERIFIED_CHANNEL_COHERENCE);
        }

        public static int? FirstMaximum(IReadOnlyList<double?> values, IEnumerable<int> indices)
        {
            int? best = null;

            foreach (var index in indices)
            {
                if (values[index] is not { } value)
                    continue;

                if (best is null || value > values[best.Value])
                    best = index;
            }

            return best;
        }
    }
}
