namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeFlightSeriesDto(long FlightId, int From, int To, IReadOnlyList<BackofficeParameterSeriesDto> Parameters);
}
