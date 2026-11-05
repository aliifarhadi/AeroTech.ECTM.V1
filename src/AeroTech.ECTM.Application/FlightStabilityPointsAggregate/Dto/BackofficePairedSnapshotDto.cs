using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficePairedSnapshotDto(
        string? MinuendKey,
        string? SubtrahendKey,
        EnumValueDto Availability,
        EnumValueDto? Reason,
        int? JointStartIndex,
        int? JointEndIndexInclusive,
        int? ReportStartIndex,
        int? ReportEndIndexInclusive,
        int MatchedSamples,
        double? MeanDifference)
    {
        public static BackofficePairedSnapshotDto From(PairedSnapshot paired)
            => new(
                paired.MinuendKey,
                paired.SubtrahendKey,
                EnumValueDto.Of(paired.Availability),
                EnumValueDto.OfNullable(paired.Reason),
                paired.JointStartIndex,
                paired.JointEndIndexInclusive,
                paired.ReportStartIndex,
                paired.ReportEndIndexInclusive,
                paired.MatchedSamples,
                paired.MeanDifference);
    }
}
