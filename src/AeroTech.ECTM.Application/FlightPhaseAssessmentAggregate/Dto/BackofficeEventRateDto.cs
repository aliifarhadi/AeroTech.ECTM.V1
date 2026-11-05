namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficeEventRateDto(
        string Code,
        int EvaluatedFlights,
        int EvaluatedLandings,
        int FlightsWithEvent,
        int SpanCount,
        double? PerHundredFlights,
        double? PerHundredLandings,
        int UnverifiedFlights,
        int MissingFlights);
}
