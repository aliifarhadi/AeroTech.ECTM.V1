using FluentValidation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates.Backoffice
{
    public sealed class BackofficeGetAircraftEventRatesQueryValidator : AbstractValidator<BackofficeGetAircraftEventRatesQuery>
    {
        public BackofficeGetAircraftEventRatesQueryValidator()
        {
            RuleFor(query => query.AircraftId).GreaterThan(0);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From).When(query => query.From is not null && query.To is not null);
        }
    }
}
