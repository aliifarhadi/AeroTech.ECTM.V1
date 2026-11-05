namespace AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests
{
    public sealed record ConfirmIdentityRequest(
        long AircraftId,
        DateOnly FlightDate,
        string FlightNumber,
        IReadOnlyList<EngineChoiceRequest> Engines,
        bool ConfirmDistinctFlight,
        string? Note);
}
