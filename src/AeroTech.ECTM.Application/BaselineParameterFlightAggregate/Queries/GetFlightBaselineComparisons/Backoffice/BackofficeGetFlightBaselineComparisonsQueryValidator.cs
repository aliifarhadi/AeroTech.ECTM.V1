using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons.Backoffice
{
    public sealed class BackofficeGetFlightBaselineComparisonsQueryValidator : AbstractValidator<BackofficeGetFlightBaselineComparisonsQuery>
    {
        public BackofficeGetFlightBaselineComparisonsQueryValidator()
        {
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.Filter.SnapshotKind).IsInEnum().When(query => query.Filter.SnapshotKind is not null);
        }
    }
}
