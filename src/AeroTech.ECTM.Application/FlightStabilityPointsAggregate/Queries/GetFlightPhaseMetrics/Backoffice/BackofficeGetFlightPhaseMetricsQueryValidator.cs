using FluentValidation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightPhaseMetrics.Backoffice
{
    public sealed class BackofficeGetFlightPhaseMetricsQueryValidator : AbstractValidator<BackofficeGetFlightPhaseMetricsQuery>
    {
        public BackofficeGetFlightPhaseMetricsQueryValidator()
        {
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.Group).IsInEnum().When(query => query.Group is not null);
        }
    }
}
