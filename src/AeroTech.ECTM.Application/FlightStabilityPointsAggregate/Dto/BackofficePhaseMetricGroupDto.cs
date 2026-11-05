using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficePhaseMetricGroupDto(
        EnumValueDto Kind,
        string Name,
        EnumValueDto? Phase,
        int? SegmentNumber,
        string? Selector,
        int? Position,
        int StartIndex,
        int EndIndexInclusive,
        IReadOnlyList<ParameterStatistics> Parameters)
    {
        public static BackofficePhaseMetricGroupDto From(PhaseMetricGroup group)
            => new(
                EnumValueDto.Of(group.Kind),
                group.Name,
                EnumValueDto.OfNullable(group.Phase),
                group.SegmentNumber,
                group.Selector,
                group.Position,
                group.StartIndex,
                group.EndIndexInclusive,
                group.Parameters);
    }
}
