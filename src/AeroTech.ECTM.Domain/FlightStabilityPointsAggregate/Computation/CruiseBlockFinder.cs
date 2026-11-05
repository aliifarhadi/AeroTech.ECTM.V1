using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class CruiseBlockFinder
    {
        public static CruiseSnapshot Find(SnapshotFrame frame, CruiseSelector selector, int position)
        {
            var gates = new CruiseWindowGates(frame, selector, position);
            var verdicts = Enumerable.Range(0, frame.Count).Select(gates.FirstFailingGate).ToList();
            var eligible = verdicts.Select(verdict => verdict is null).ToList();
            var rejected = verdicts
                .OfType<CruiseGate>()
                .GroupBy(gate => gate)
                .OrderBy(group => group.Key)
                .ToDictionary(group => group.Key, group => group.Count());
            if (LongestBlock(frame, selector, eligible) is not { } block)
                return NotEvaluable(selector, position, SnapshotReason.NO_ELIGIBLE_BLOCK, new CruiseGateTrace(0, rejected, [], []));

            var (blockStart, blockEnd, firstEnd, lastEnd) = block;
            var nearestRejected = NearestRejected(verdicts, firstEnd, lastEnd);
            var trace = new CruiseGateTrace(eligible.Count(isEligible => isEligible), rejected, [], nearestRejected);
            var reportSamples = selector.ReportSec!.Value;
            var center = (blockStart + blockEnd) / 2;
            var reportStart = center - reportSamples / 2;
            var reportEnd = reportStart + reportSamples - 1;
            var blockSeconds = frame.Elapsed[blockEnd] - frame.Elapsed[blockStart] + 1;

            if (reportStart < blockStart || reportEnd > blockEnd)
                return WithBlock(selector, position, SnapshotReason.REPORT_WINDOW_OUTSIDE_BLOCK, blockStart, blockEnd, blockSeconds, trace);

            var report = frame.Range(reportStart, reportEnd).ToList();
            var values = frame.Rules.EngineParameters!
                .Select(code => frame.SummaryOf(SnapshotFrame.KeyOf(code, position), report))
                .ToList();
            var egt = values.Single(value => value.ParameterKey == SnapshotFrame.KeyOf(frame.Rules.Channels!.EngineEgt!, position));
            var measuredTrace = trace with { ReportWindowGates = gates.Measure(reportStart, reportEnd) };

            if (egt.Mean is null)
                return WithBlock(selector, position, SnapshotReason.NO_EGT_IN_SELECTED_WINDOW, blockStart, blockEnd, blockSeconds, measuredTrace);

            return new CruiseSnapshot(
                selector.Code!,
                position,
                SnapshotAvailability.AVAILABLE,
                null,
                blockStart,
                blockEnd,
                blockSeconds,
                reportStart,
                reportEnd,
                values,
                SnapshotContext.Of(frame, report),
                measuredTrace);
        }

        private static (int Start, int End, int FirstWindowEnd, int LastWindowEnd)? LongestBlock(SnapshotFrame frame, CruiseSelector selector, IReadOnlyList<bool> eligible)
        {
            (int Start, int End, int FirstWindowEnd, int LastWindowEnd)? best = null;
            var bestSeconds = 0.0;

            foreach (var (firstEnd, lastEnd) in frame.RunsOf(eligible))
            {
                var start = firstEnd - selector.WindowSec!.Value + 1;
                var seconds = frame.Elapsed[lastEnd] - frame.Elapsed[start] + 1;

                if (seconds < selector.MinBlockSec || (best is not null && seconds <= bestSeconds))
                    continue;

                best = (start, lastEnd, firstEnd, lastEnd);
                bestSeconds = seconds;
            }

            return best;
        }

        private static IReadOnlyList<RejectedWindow> NearestRejected(IReadOnlyList<CruiseGate?> verdicts, int firstEnd, int lastEnd)
        {
            var nearest = new List<RejectedWindow>();

            if (firstEnd - 1 >= 0 && verdicts[firstEnd - 1] is { } before)
                nearest.Add(new RejectedWindow(firstEnd - 1, before));

            if (lastEnd + 1 < verdicts.Count && verdicts[lastEnd + 1] is { } after)
                nearest.Add(new RejectedWindow(lastEnd + 1, after));

            return nearest;
        }

        private static CruiseSnapshot NotEvaluable(CruiseSelector selector, int position, SnapshotReason reason, CruiseGateTrace trace)
            => new(selector.Code!, position, SnapshotAvailability.NOT_EVALUABLE, reason, null, null, null, null, null, [], null, trace);

        private static CruiseSnapshot WithBlock(CruiseSelector selector, int position, SnapshotReason reason, int blockStart, int blockEnd, double blockSeconds, CruiseGateTrace trace)
            => new(selector.Code!, position, SnapshotAvailability.NOT_EVALUABLE, reason, blockStart, blockEnd, blockSeconds, null, null, [], null, trace);
    }
}
