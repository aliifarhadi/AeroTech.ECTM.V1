namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeAircraftEventRatesDto(
        long AircraftId,
        string Registration,
        DateOnly? From,
        DateOnly? To,
        int Flights,
        int FlightsWithPhases,
        int Landings,
        IReadOnlyList<BackofficeEventRateDto> Events);
}
