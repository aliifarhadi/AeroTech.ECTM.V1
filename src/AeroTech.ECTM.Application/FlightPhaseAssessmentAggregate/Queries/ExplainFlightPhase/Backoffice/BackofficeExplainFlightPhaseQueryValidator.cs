using FluentValidation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.ExplainFlightPhase.Backoffice
{
    public sealed class BackofficeExplainFlightPhaseQueryValidator : AbstractValidator<BackofficeExplainFlightPhaseQuery>
    {
        public BackofficeExplainFlightPhaseQueryValidator()
        {
            RuleFor(query => query.FlightId).GreaterThan(0);
            RuleFor(query => query.SampleIndex).GreaterThanOrEqualTo(0);
        }
    }
}
