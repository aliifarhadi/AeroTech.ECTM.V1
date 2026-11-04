namespace AeroTech.ECTM.RestApi.V1.EngineAggregate.Requests
{
    public sealed record SetLifeCountersRequest(
        decimal? HoursSinceNew,
        int? CyclesSinceNew,
        decimal? HoursSinceLastEvent,
        int? CyclesSinceLastEvent,
        DateTimeOffset BasisUtc,
        string Reason);
}
