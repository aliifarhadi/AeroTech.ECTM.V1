using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record SnapshotReading(
        SnapshotKind Kind,
        string? Selector,
        int Position,
        SnapshotAvailability Availability,
        SnapshotReason? Reason,
        int? StartIndex,
        int? EndIndexInclusive,
        int? PeakIndex,
        IReadOnlyList<ParameterSummary> Values,
        IReadOnlyList<ParameterSummary> Conditions,
        IReadOnlyList<string> Configurations,
        SnapshotContextStatus? ContextStatus)
    {
        public bool IsAvailable => Availability == SnapshotAvailability.AVAILABLE;

        public bool HasContext => Conditions.Count > 0 || Configurations.Count > 0;

        public string? SingleConfiguration => Configurations.Count == 1 ? Configurations[0] : null;

        public static IReadOnlyList<SnapshotReading> AllOf(StabilityPointsResult result, int position)
        {
            var readings = new List<SnapshotReading>();

            if (TakeoffN1PeakOf(result, position) is { } n1Peak)
                readings.Add(n1Peak);

            if (TakeoffEgtPeakOf(result, position) is { } egtPeak)
                readings.Add(egtPeak);

            foreach (var selector in result.Cruise.Select(cruise => cruise.Selector))
            {
                if (CruiseOf(result, selector, position) is { } cruise)
                    readings.Add(cruise);
            }

            return readings;
        }

        public static SnapshotReading? Of(StabilityPointsResult result, SnapshotKind kind, string? selector, int position)
            => kind switch
            {
                SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT => TakeoffN1PeakOf(result, position),
                SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED => TakeoffEgtPeakOf(result, position),
                _ => CruiseOf(result, selector, position)
            };

        public double? MeanOf(string parameterCode)
        {
            var key = SnapshotFrame.KeyOf(parameterCode, Position);

            return Values.FirstOrDefault(value => value.ParameterKey == key)?.Mean;
        }

        private static SnapshotReading? TakeoffN1PeakOf(StabilityPointsResult result, int position)
        {
            var peak = result.TakeoffN1Peaks.FirstOrDefault(candidate => candidate.Position == position);

            if (peak is null)
                return null;

            return new SnapshotReading(
                SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT,
                null,
                position,
                peak.Availability,
                peak.Reason,
                peak.StartIndex,
                peak.EndIndexInclusive,
                peak.PeakIndex,
                peak.Values,
                peak.Context?.Conditions ?? [],
                peak.Context?.Configurations ?? [],
                peak.ContextStatus);
        }

        private static SnapshotReading? TakeoffEgtPeakOf(StabilityPointsResult result, int position)
        {
            var peak = result.TakeoffEgtPeaks.FirstOrDefault(candidate => candidate.Position == position);

            if (peak is null)
                return null;

            return new SnapshotReading(
                SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED,
                null,
                position,
                peak.Availability,
                peak.Reason,
                peak.PeakIndex,
                peak.PeakIndex,
                peak.PeakIndex,
                peak.ValuesAtPeak,
                [],
                [],
                null);
        }

        private static SnapshotReading? CruiseOf(StabilityPointsResult result, string? selector, int position)
        {
            var engine = result.Cruise
                .Where(cruise => cruise.Selector == selector)
                .SelectMany(cruise => cruise.Engines)
                .FirstOrDefault(candidate => candidate.Position == position);

            if (engine is null)
                return null;

            return new SnapshotReading(
                SnapshotKind.CRUISE,
                engine.Selector,
                position,
                engine.Availability,
                engine.Reason,
                engine.ReportStartIndex,
                engine.ReportEndIndexInclusive,
                null,
                engine.Values,
                engine.Context?.Conditions ?? [],
                engine.Context?.Configurations ?? [],
                null);
        }
    }
}
