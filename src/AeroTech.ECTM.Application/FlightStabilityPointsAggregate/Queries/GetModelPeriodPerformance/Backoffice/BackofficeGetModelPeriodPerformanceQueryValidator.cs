using AeroTech.Messages.Ectm.Enums;
using FluentValidation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance.Backoffice
{
    public sealed class BackofficeGetModelPeriodPerformanceQueryValidator : AbstractValidator<BackofficeGetModelPeriodPerformanceQuery>
    {
        public BackofficeGetModelPeriodPerformanceQueryValidator()
        {
            RuleFor(query => query.EngineModelId).GreaterThan(0);
            RuleFor(query => query.Kind).IsInEnum();
            RuleFor(query => query.Selector).NotEmpty().When(query => query.Kind == SnapshotKind.CRUISE);
            RuleFor(query => query.To).GreaterThanOrEqualTo(query => query.From).When(query => query.From is not null && query.To is not null);
        }
    }
}
