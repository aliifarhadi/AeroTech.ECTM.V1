namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightSamplesDto(long FlightId, int From, int To, IReadOnlyList<string> Header, IReadOnlyList<BackofficeSampleRowDto> Rows);
}
