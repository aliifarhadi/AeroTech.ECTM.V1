using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.EngineAggregate.Entities;

namespace AeroTech.ECTM.Application.EngineAggregate.Dto
{
    public sealed record BackofficeLifeCounterAdjustmentDto(
        long Id,
        decimal? HoursSinceNew,
        int? CyclesSinceNew,
        decimal? HoursSinceLastEvent,
        int? CyclesSinceLastEvent,
        DateTimeOffset BasisUtc,
        EnumValueDto Source,
        string Reason,
        DateTimeOffset AdjustedUtc,
        string? AdjustedBy)
    {
        public static BackofficeLifeCounterAdjustmentDto From(LifeCounterAdjustment adjustment)
            => new(
                adjustment.Id,
                adjustment.HoursSinceNew,
                adjustment.CyclesSinceNew,
                adjustment.HoursSinceLastEvent,
                adjustment.CyclesSinceLastEvent,
                adjustment.BasisAt,
                EnumValueDto.Of(adjustment.Source),
                adjustment.Reason,
                adjustment.AdjustedAt,
                adjustment.AdjustedBy);
    }
}
