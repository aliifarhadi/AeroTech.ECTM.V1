using AeroTech.ECTM.Domain.EngineAggregate.Arguments;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands
{
    public sealed record LifeCountersInput(
        decimal? HoursSinceNew,
        int? CyclesSinceNew,
        decimal? HoursSinceLastEvent,
        int? CyclesSinceLastEvent,
        DateTimeOffset BasisUtc)
    {
        public LifeCounterArgs ToArgs()
            => new(HoursSinceNew, CyclesSinceNew, HoursSinceLastEvent, CyclesSinceLastEvent, BasisUtc);
    }
}
