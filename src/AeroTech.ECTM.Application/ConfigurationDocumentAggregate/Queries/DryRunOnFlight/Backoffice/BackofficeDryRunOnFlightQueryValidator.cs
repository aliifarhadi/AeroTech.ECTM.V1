using AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight.Backoffice
{
    public sealed class BackofficeDryRunOnFlightQueryValidator : AbstractValidator<BackofficeDryRunOnFlightQuery>
    {
        public BackofficeDryRunOnFlightQueryValidator()
        {
            RuleFor(query => query.Kind).NotEmpty();
            RuleFor(query => query.DocumentId).GreaterThan(0);
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.Budget).InclusiveBetween(M4Reducer.PointsPerBucket, GetFlightSeriesService.MaximumBudget);
        }
    }
}
