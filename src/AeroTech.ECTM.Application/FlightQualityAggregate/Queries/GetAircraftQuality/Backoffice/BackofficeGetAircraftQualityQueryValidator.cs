using FluentValidation;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality.Backoffice
{
    public sealed class BackofficeGetAircraftQualityQueryValidator : AbstractValidator<BackofficeGetAircraftQualityQuery>
    {
        public BackofficeGetAircraftQualityQueryValidator()
        {
            RuleFor(query => query.AircraftId).GreaterThan(0);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From).When(query => query.From is not null && query.To is not null);
        }
    }
}
