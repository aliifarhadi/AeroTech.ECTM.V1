using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.RestApi.V1.FlightAggregate.Requests
{
    public sealed record ReviewFlightRequest(ReviewAction Action, string? Note);
}
