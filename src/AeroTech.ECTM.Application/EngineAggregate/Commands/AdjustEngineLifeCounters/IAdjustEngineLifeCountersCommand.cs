namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters
{
    public interface IAdjustEngineLifeCountersCommand
    {
        long EngineId { get; }

        LifeCountersInput LifeCounters { get; }

        string Reason { get; }
    }
}
