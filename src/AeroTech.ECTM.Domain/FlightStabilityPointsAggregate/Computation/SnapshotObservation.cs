using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed record SnapshotObservation(IReadOnlyList<ParameterSummary> Values, IReadOnlyList<ParameterSummary> Conditions, IReadOnlyList<string> Configurations)
    {
        public static SnapshotObservation? Of(StabilityPointsResult result, int position, SnapshotKind kind, string? selector)
        {
            switch (kind)
            {
                case SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT:
                {
                    var peak = result.TakeoffN1Peaks.FirstOrDefault(candidate => candidate.Position == position);

                    if (peak is not { Availability: SnapshotAvailability.AVAILABLE, Context: { } context })
                        return null;

                    return new SnapshotObservation(peak.Values, context.Conditions, context.Configurations);
                }
                case SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED:
                {
                    var peak = result.TakeoffEgtPeaks.FirstOrDefault(candidate => candidate.Position == position);

                    if (peak is not { Availability: SnapshotAvailability.AVAILABLE })
                        return null;

                    return new SnapshotObservation(peak.ValuesAtPeak, [], []);
                }
                default:
                {
                    var engine = result.Cruise
                        .Where(candidate => candidate.Selector == selector)
                        .SelectMany(candidate => candidate.Engines)
                        .FirstOrDefault(candidate => candidate.Position == position);

                    if (engine is not { Availability: SnapshotAvailability.AVAILABLE, Context: { } context })
                        return null;

                    return new SnapshotObservation(engine.Values, context.Conditions, context.Configurations);
                }
            }
        }
    }
}
