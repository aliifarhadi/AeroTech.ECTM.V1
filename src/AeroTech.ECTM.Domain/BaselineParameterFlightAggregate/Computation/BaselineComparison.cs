using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record BaselineComparison(
        ComparisonStatus Status,
        double? RawObserved,
        double? RawDelta,
        IReadOnlyList<ComparisonReason> Reasons,
        ComparisonDetail Detail)
    {
        public static BaselineComparison NotComparable(ComparisonReason reason, double? rawObserved, ComparisonDetail detail)
            => new(ComparisonStatus.NOT_COMPARABLE, rawObserved, null, [reason, ComparisonReason.NO_APPROVED_CORRECTION_MODEL], detail);
    }
}
