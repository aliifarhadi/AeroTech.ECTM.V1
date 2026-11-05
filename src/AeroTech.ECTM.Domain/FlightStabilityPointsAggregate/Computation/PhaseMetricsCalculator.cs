using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class PhaseMetricsCalculator
    {
        public static IReadOnlyList<PhaseMetricGroup> Calculate(SnapshotFrame frame, IReadOnlyList<CruiseSelectorResult> cruise)
        {
            var allKeys = EngineKeys(frame, frame.Positions).Concat(frame.Rules.ConditionParameters!).ToList();
            var groups = new List<PhaseMetricGroup>
            {
                GroupOf(frame, MetricGroupKind.WHOLE_FLIGHT, "WHOLE_FLIGHT", Enumerable.Range(0, frame.Count).ToList(), allKeys)
            };

            foreach (var phase in frame.Phases.Segments.Select(segment => segment.PhaseCode).Distinct().Order())
            {
                var indices = frame.Phases.Segments
                    .Where(segment => segment.PhaseCode == phase)
                    .SelectMany(segment => frame.Range(segment.StartIndex, segment.EndIndexInclusive))
                    .ToList();

                groups.Add(GroupOf(frame, MetricGroupKind.PHASE, phase.ToString(), indices, allKeys) with { Phase = phase });
            }

            for (var number = 1; number <= frame.Phases.Segments.Count; number++)
            {
                var segment = frame.Phases.Segments[number - 1];
                var indices = frame.Range(segment.StartIndex, segment.EndIndexInclusive).ToList();

                groups.Add(GroupOf(frame, MetricGroupKind.SEGMENT, $"SEGMENT {number} {segment.PhaseCode}", indices, allKeys) with
                {
                    Phase = segment.PhaseCode,
                    SegmentNumber = number
                });
            }

            foreach (var engine in cruise.SelectMany(selector => selector.Engines).Where(engine => engine.HasBlock))
            {
                var indices = frame.Range(engine.BlockStartIndex!.Value, engine.BlockEndIndexInclusive!.Value).ToList();
                var keys = EngineKeys(frame, [engine.Position]).Concat(frame.Rules.ConditionParameters!).ToList();

                groups.Add(GroupOf(frame, MetricGroupKind.STEADY_CRUISE, $"STEADY_CRUISE {engine.Selector} engine {engine.Position}", indices, keys) with
                {
                    Selector = engine.Selector,
                    Position = engine.Position
                });
            }

            return groups;
        }

        private static IEnumerable<string> EngineKeys(SnapshotFrame frame, IEnumerable<int> positions)
            => positions.SelectMany(position => frame.Rules.EngineParameters!.Select(code => SnapshotFrame.KeyOf(code, position)));

        private static PhaseMetricGroup GroupOf(SnapshotFrame frame, MetricGroupKind kind, string name, IReadOnlyList<int> indices, IReadOnlyList<string> keys)
        {
            var parameters = keys.Select(key => StatisticsOf(frame, key, indices)).ToList();

            return new PhaseMetricGroup(kind, name, null, null, null, null, indices[0], indices[^1], parameters);
        }

        private static ParameterStatistics StatisticsOf(SnapshotFrame frame, string key, IReadOnlyList<int> indices)
        {
            var source = frame.Numbers(key);
            var values = indices.Select(index => source[index]).OfType<double>().ToList();
            var sorted = values.Order().ToList();
            var gapStarts = indices.Skip(1).Count(frame.StartsAfterGap);
            var masked = indices.Count(frame.IsMasked);
            var percentile = frame.Rules.PhaseMetrics!.Percentile!.Value;

            return new ParameterStatistics(
                key,
                indices.Count,
                values.Count,
                indices.Count - values.Count,
                gapStarts,
                masked,
                sorted.Count == 0 ? null : sorted[0],
                sorted.Count == 0 ? null : sorted[^1],
                SampleStatistics.Mean(values),
                SampleStatistics.Median(sorted),
                SampleStatistics.NearestRankPercentile(sorted, percentile),
                SampleStatistics.SampleStandardDeviation(values));
        }
    }
}
