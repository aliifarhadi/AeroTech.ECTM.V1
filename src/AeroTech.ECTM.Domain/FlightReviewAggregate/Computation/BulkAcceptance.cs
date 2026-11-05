using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightReviewAggregate.Computation
{
    public static class BulkAcceptance
    {
        public const string NotPendingReview = "NOT_PENDING_REVIEW";
        public const string IdentityNotResolved = "IDENTITY_NOT_RESOLVED";
        public const string GradeNotAOrB = "GRADE_NOT_A_OR_B";
        public const string BlockingValidation = "BLOCKING_VALIDATION";
        public const string SnapshotNotAvailable = "SNAPSHOT_NOT_AVAILABLE";

        public static IReadOnlyList<AcceptanceRefusal> RefusalsOf(
            FlightState state,
            IdentityStatus identity,
            QualityGrade? grade,
            int blockingCount,
            StabilityPointsResult? points)
        {
            var refusals = new List<AcceptanceRefusal>();

            if (state != FlightState.PENDING_REVIEW)
                refusals.Add(new AcceptanceRefusal(NotPendingReview));

            if (identity != IdentityStatus.RESOLVED)
                refusals.Add(new AcceptanceRefusal(IdentityNotResolved));

            if (grade is not (QualityGrade.A or QualityGrade.B))
                refusals.Add(new AcceptanceRefusal(GradeNotAOrB));

            if (blockingCount > 0)
                refusals.Add(new AcceptanceRefusal(BlockingValidation));

            refusals.AddRange(MissingSnapshots(points));

            return refusals;
        }

        private static IEnumerable<AcceptanceRefusal> MissingSnapshots(StabilityPointsResult? points)
        {
            if (points is null || points.NotEvaluableReason is not null)
            {
                yield return new AcceptanceRefusal(SnapshotNotAvailable);
                yield break;
            }

            foreach (var position in points.Positions)
            {
                if (!points.TakeoffN1Peaks.Any(peak => peak.Position == position && peak.Availability == SnapshotAvailability.AVAILABLE))
                    yield return new AcceptanceRefusal(SnapshotNotAvailable, SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT, position);

                if (!points.TakeoffEgtPeaks.Any(peak => peak.Position == position && peak.Availability == SnapshotAvailability.AVAILABLE))
                    yield return new AcceptanceRefusal(SnapshotNotAvailable, SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED, position);

                var cruiseAvailable = points.Cruise
                    .SelectMany(selector => selector.Engines)
                    .Any(engine => engine.Position == position && engine.Availability == SnapshotAvailability.AVAILABLE);

                if (!cruiseAvailable)
                    yield return new AcceptanceRefusal(SnapshotNotAvailable, SnapshotKind.CRUISE, position);
            }
        }
    }
}
