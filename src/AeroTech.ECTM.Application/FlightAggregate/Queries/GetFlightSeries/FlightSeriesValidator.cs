using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using FluentValidation;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries
{
    public abstract class FlightSeriesValidator<TQuery> : AbstractValidator<TQuery>
        where TQuery : IFlightSeriesQuery
    {
        protected FlightSeriesValidator()
        {
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.Parameters).NotEmpty().MaximumLength(1024);
            RuleFor(query => query.Parameters)
                .Must(parameters => GetFlightSeriesService.KeysOf(parameters).Count <= GetFlightSeriesService.MaximumParameters)
                .WithMessage($"At most {GetFlightSeriesService.MaximumParameters} parameters can be charted at once.");
            RuleFor(query => query.Sampling).Must(sampling => sampling is SeriesSampler.M4 or SeriesSampler.Raw)
                .WithMessage($"Sampling is {SeriesSampler.M4} or {SeriesSampler.Raw}.");
            RuleFor(query => query.Budget).InclusiveBetween(M4Reducer.PointsPerBucket, GetFlightSeriesService.MaximumBudget);
            RuleFor(query => query.From).GreaterThanOrEqualTo(0);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From ?? 0).When(query => query.To is not null);
        }
    }
}
