namespace AeroTech.ECTM.Domain.EngineAggregate.Arguments
{
    public sealed record LifeCounterArgs(
        decimal? HoursSinceNew,
        int? CyclesSinceNew,
        decimal? HoursSinceLastEvent,
        int? CyclesSinceLastEvent,
        DateTimeOffset BasisAt);
}
