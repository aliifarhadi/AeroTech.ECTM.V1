namespace AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests
{
    public sealed record EngineChoiceRequest(int Position, long? EngineId, bool PositionOnly);
}
