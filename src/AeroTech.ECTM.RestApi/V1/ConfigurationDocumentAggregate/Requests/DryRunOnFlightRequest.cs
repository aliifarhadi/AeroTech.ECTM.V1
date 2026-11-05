namespace AeroTech.ECTM.RestApi.V1.ConfigurationDocumentAggregate.Requests
{
    public sealed record DryRunOnFlightRequest(long FlightId, int? Budget);
}
