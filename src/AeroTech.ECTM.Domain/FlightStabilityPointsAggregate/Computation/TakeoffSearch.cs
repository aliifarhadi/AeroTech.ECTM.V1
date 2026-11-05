using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record TakeoffSearch(int StartIndex, int EndIndexInclusive, TakeoffSearchBasis Basis)
    {
        public static TakeoffSearch? Find(SnapshotFrame frame, int position)
        {
            var takeoff = frame.Rules.Takeoff!;

            if (frame.AnchorIndex(takeoff.LiftoffAnchor!) is { } liftoff)
            {
                var start = frame.FirstAtOrAfter(frame.Elapsed[liftoff] - takeoff.SearchBeforeLiftoffSec!.Value);
                var end = frame.LastAtOrBefore(frame.Elapsed[liftoff] + takeoff.SearchAfterLiftoffSec!.Value);

                return new TakeoffSearch(start, end, TakeoffSearchBasis.LIFTOFF_ANCHORED);
            }

            if (!frame.PartialStart || FirstHighN1(frame, position) is not { } firstHigh)
                return null;

            var partialEnd = frame.LastAtOrBefore(frame.Elapsed[firstHigh] + takeoff.SearchAfterLiftoffSec!.Value);

            return new TakeoffSearch(0, partialEnd, TakeoffSearchBasis.PARTIAL_START_FIRST_HIGH_N1);
        }

        public IEnumerable<int> SamplesInBlockOf(SnapshotFrame frame, int blockSample)
            => frame.Range(StartIndex, EndIndexInclusive).Where(index => frame.SameBlock(index, blockSample));

        private static int? FirstHighN1(SnapshotFrame frame, int position)
        {
            var n1 = frame.Numbers(SnapshotFrame.KeyOf(frame.Rules.Channels!.EngineN1!, position));
            var threshold = frame.Rules.Takeoff!.PartialStartN1Pct!.Value;

            for (var index = 0; index < frame.Count; index++)
            {
                if (n1[index] >= threshold)
                    return index;
            }

            return null;
        }
    }
}
