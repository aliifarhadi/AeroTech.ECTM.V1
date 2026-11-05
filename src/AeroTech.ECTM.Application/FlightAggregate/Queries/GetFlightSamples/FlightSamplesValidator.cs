using FluentValidation;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples
{
    public abstract class FlightSamplesValidator<TQuery> : AbstractValidator<TQuery>
        where TQuery : IFlightSamplesQuery
    {
        protected FlightSamplesValidator()
        {
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.From).GreaterThanOrEqualTo(0);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From ?? 0).When(query => query.To is not null);
            RuleFor(query => query.To)
                .Must((query, to) => to!.Value - (query.From ?? 0) + 1 <= GetFlightSamplesService.MaximumRows)
                .When(query => query.To is not null)
                .WithMessage($"At most {GetFlightSamplesService.MaximumRows} rows are returned at once.");
        }
    }
}
