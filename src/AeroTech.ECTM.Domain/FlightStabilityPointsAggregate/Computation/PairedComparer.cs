using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class PairedComparer
    {
        private const int PairedPositions = 2;

        public static PairedSnapshot Compare(SnapshotFrame frame, CruiseSelector selector, IReadOnlyList<CruiseSnapshot> engines)
        {
            if (engines.Count != PairedPositions || !engines.All(engine => engine.HasBlock))
                return NotEvaluable(selector, SnapshotReason.MISSING_ENGINE_WINDOW, null, null);

            var subtrahend = engines[0];
            var minuend = engines[1];
            var parameter = frame.Rules.Paired!.Parameter!;
            var minuendKey = SnapshotFrame.KeyOf(parameter, minuend.Position);
            var subtrahendKey = SnapshotFrame.KeyOf(parameter, subtrahend.Position);
            var jointStart = Math.Max(subtrahend.BlockStartIndex!.Value, minuend.BlockStartIndex!.Value);
            var jointEnd = Math.Min(subtrahend.BlockEndIndexInclusive!.Value, minuend.BlockEndIndexInclusive!.Value);
            var reportSamples = selector.ReportSec!.Value;

            var jointLength = jointEnd - jointStart + 1;

            if (jointLength < reportSamples)
                return NotEvaluable(selector, SnapshotReason.INSUFFICIENT_JOINT_BLOCK, minuendKey, subtrahendKey) with
                {
                    JointStartIndex = jointStart,
                    JointEndIndexInclusive = jointEnd
                };

            var reportStart = jointStart + (jointLength - reportSamples) / 2;
            var reportEnd = reportStart + reportSamples - 1;
            var differences = Differences(frame.Numbers(minuendKey), frame.Numbers(subtrahendKey), reportStart, reportEnd);
            var compared = new PairedSnapshot(
                selector.Code!,
                minuendKey,
                subtrahendKey,
                SnapshotAvailability.AVAILABLE,
                null,
                jointStart,
                jointEnd,
                reportStart,
                reportEnd,
                differences.Count,
                null,
                null);

            if (differences.Count < frame.Rules.Paired.MinMatchedSamples)
                return compared with { Availability = SnapshotAvailability.NOT_EVALUABLE, Reason = SnapshotReason.INSUFFICIENT_SAME_INDEX_OVERLAP };

            var sum = CompensatedSum.Of(differences);

            return compared with { SumDifference = sum, MeanDifference = sum / differences.Count };
        }

        private static List<double> Differences(IReadOnlyList<double?> minuend, IReadOnlyList<double?> subtrahend, int start, int end)
        {
            var differences = new List<double>();

            for (var index = start; index <= end; index++)
            {
                if (minuend[index] is { } first && subtrahend[index] is { } second)
                    differences.Add(first - second);
            }

            return differences;
        }

        private static PairedSnapshot NotEvaluable(CruiseSelector selector, SnapshotReason reason, string? minuendKey, string? subtrahendKey)
            => new(selector.Code!, minuendKey, subtrahendKey, SnapshotAvailability.NOT_EVALUABLE, reason, null, null, null, null, 0, null, null);
    }
}
