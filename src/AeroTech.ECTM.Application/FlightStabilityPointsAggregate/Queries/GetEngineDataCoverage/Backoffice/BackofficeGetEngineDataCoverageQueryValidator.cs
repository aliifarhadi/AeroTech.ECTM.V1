using FluentValidation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage.Backoffice
{
    public sealed class BackofficeGetEngineDataCoverageQueryValidator : AbstractValidator<BackofficeGetEngineDataCoverageQuery>
    {
        public BackofficeGetEngineDataCoverageQueryValidator()
        {
            RuleFor(query => query.EngineId).GreaterThan(0);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From).When(query => query.From is not null && query.To is not null);
        }
    }
}
