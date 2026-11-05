using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates
{
    public sealed class GetAircraftEventRatesService : IGetAircraftEventRatesService
    {
        public const string LandingAnchor = "TOUCHDOWN";

        private readonly IAircraftRepository _aircraft;
        private readonly IFlightRepository _flights;
        private readonly IFlightPhaseAssessmentRepository _phases;

        public GetAircraftEventRatesService(IAircraftRepository aircraft, IFlightRepository flights, IFlightPhaseAssessmentRepository phases)
        {
            _aircraft = aircraft;
            _flights = flights;
            _phases = phases;
        }

        public async Task<BackofficeAircraftEventRatesDto> ExecuteAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var aircraft = await _aircraft.GetAsync(aircraftId, cancellationToken)
                           ?? throw ExceptionFactory.AircraftNotFound(aircraftId);
            var flights = await _flights.ListForAircraftAsync(aircraft.Id, from, to, cancellationToken);
            var assessments = await _phases.ListCurrentAsync(flights.Select(flight => flight.Id).ToList(), cancellationToken);
            var results = assessments
                .Select(assessment => assessment.ReadResult())
                .Where(result => result.IsEvaluated)
                .ToList();
            var codes = results
                .SelectMany(result => result.EventCoverage.Select(coverage => coverage.Code))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            return new BackofficeAircraftEventRatesDto(
                aircraft.Id,
                aircraft.Registration,
                from,
                to,
                flights.Count,
                results.Count,
                results.Count(HasLanding),
                codes.Select(code => RateOf(code, results)).ToList());
        }

        private static BackofficeEventRateDto RateOf(string code, IReadOnlyList<PhaseResult> results)
        {
            var evaluated = results.Where(result => HasCoverage(result, code, EventEvaluability.EVALUATED)).ToList();
            var evaluatedLandings = evaluated.Count(HasLanding);
            var flightsWithEvent = evaluated.Count(result => result.Events.Any(span => span.Code == code));
            var spanCount = evaluated.Sum(result => result.Events.Count(span => span.Code == code));
            var unverified = results.Count(result => OnlyCoverage(result, code, EventEvaluability.UNVERIFIED_ENUM));
            var missing = results.Count(result => OnlyCoverage(result, code, EventEvaluability.MISSING));

            return new BackofficeEventRateDto(
                code,
                evaluated.Count,
                evaluatedLandings,
                flightsWithEvent,
                spanCount,
                PerHundred(flightsWithEvent, evaluated.Count),
                PerHundred(flightsWithEvent, evaluatedLandings),
                unverified,
                missing);
        }

        private static bool HasLanding(PhaseResult result) => result.AnchorOf(LandingAnchor)?.Index is not null;

        private static bool HasCoverage(PhaseResult result, string code, EventEvaluability evaluability)
            => result.EventCoverage.Any(coverage => coverage.Code == code && coverage.Evaluability == evaluability);

        private static bool OnlyCoverage(PhaseResult result, string code, EventEvaluability evaluability)
        {
            var entries = result.EventCoverage.Where(coverage => coverage.Code == code).ToList();

            return entries.Count > 0 && entries.All(coverage => coverage.Evaluability == evaluability);
        }

        private static double? PerHundred(int count, int denominator)
            => denominator == 0 ? null : Math.Round(100.0 * count / denominator, 2);
    }
}
