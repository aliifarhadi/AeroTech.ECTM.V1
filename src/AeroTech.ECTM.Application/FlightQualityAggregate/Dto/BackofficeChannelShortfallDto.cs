namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeChannelShortfallDto(string ParameterKey, int FlightsNotRecorded, int FlightsBelowFullCoverage);
}
