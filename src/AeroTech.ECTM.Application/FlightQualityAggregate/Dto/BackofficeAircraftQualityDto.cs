namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeAircraftQualityDto(
        long AircraftId,
        string Registration,
        DateOnly? From,
        DateOnly? To,
        int Flights,
        int Evaluated,
        IReadOnlyList<BackofficeGradeCountDto> Grades,
        int UntrustedTime,
        int ClockDisagreement,
        int WithDiscontinuity,
        int PartialStart,
        int PartialEnd,
        IReadOnlyList<BackofficeChannelShortfallDto> ChannelShortfalls,
        IReadOnlyList<BackofficeFlightQualityRowDto> Rows);
}
