using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.EngineAggregate;

namespace AeroTech.ECTM.Application.EngineAggregate.Dto
{
    public sealed record BackofficeLifeCountersDto(
        decimal? HoursSinceNew,
        int? CyclesSinceNew,
        decimal? HoursSinceLastEvent,
        int? CyclesSinceLastEvent,
        DateTimeOffset BasisUtc,
        EnumValueDto Source)
    {
        public static BackofficeLifeCountersDto? From(Engine engine)
        {
            if (engine.LifeCountersBasisAt is not { } basisAt)
                return null;

            if (engine.LifeCountersSource is not { } source)
                return null;

            return new(
                engine.HoursSinceNew,
                engine.CyclesSinceNew,
                engine.HoursSinceLastEvent,
                engine.CyclesSinceLastEvent,
                basisAt,
                EnumValueDto.Of(source));
        }
    }
}
