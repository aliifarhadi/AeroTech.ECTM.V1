namespace AeroTech.ECTM.Domain.FlightAggregate.Arguments
{
    public sealed record EngineChoiceArgs(int Position, long? EngineId, bool PositionOnly);
}
