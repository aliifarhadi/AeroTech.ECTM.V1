using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunPhasesOnFlight.Backoffice
{
    public sealed class BackofficeDryRunPhasesOnFlightQueryValidator : AbstractValidator<BackofficeDryRunPhasesOnFlightQuery>
    {
        public BackofficeDryRunPhasesOnFlightQueryValidator()
        {
            RuleFor(query => query.Kind).NotEmpty();
            RuleFor(query => query.DocumentId).GreaterThan(0);
            RuleFor(query => query.FlightId).GreaterThan(0);
        }
    }
}
