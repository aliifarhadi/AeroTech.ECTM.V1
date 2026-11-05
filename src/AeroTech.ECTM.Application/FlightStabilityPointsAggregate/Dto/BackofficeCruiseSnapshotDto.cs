using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeCruiseSnapshotDto(
        int Position,
        EnumValueDto Availability,
        EnumValueDto? Reason,
        int? BlockStartIndex,
        int? BlockEndIndexInclusive,
        double? BlockSeconds,
        int? ReportStartIndex,
        int? ReportEndIndexInclusive,
        IReadOnlyList<ParameterSummary> Values,
        SnapshotContext? Context,
        BackofficeCruiseGateTraceDto Gates)
    {
        public static BackofficeCruiseSnapshotDto From(CruiseSnapshot snapshot)
            => new(
                snapshot.Position,
                EnumValueDto.Of(snapshot.Availability),
                EnumValueDto.OfNullable(snapshot.Reason),
                snapshot.BlockStartIndex,
                snapshot.BlockEndIndexInclusive,
                snapshot.BlockSeconds,
                snapshot.ReportStartIndex,
                snapshot.ReportEndIndexInclusive,
                snapshot.Values,
                snapshot.Context,
                BackofficeCruiseGateTraceDto.From(snapshot.Gates));
    }
}
