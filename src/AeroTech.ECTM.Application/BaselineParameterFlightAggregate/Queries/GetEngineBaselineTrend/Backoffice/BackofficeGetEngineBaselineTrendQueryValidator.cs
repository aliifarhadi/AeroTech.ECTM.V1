using AeroTech.ECTM.Application._Shared.Paging;
using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend.Backoffice
{
    public sealed class BackofficeGetEngineBaselineTrendQueryValidator : AbstractValidator<BackofficeGetEngineBaselineTrendQuery>
    {
        public BackofficeGetEngineBaselineTrendQueryValidator()
        {
            RuleFor(query => query.EngineId).GreaterThan(0);
            RuleFor(query => query.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(query => query.PageSize).InclusiveBetween(1, PageLimits.MaxPageSize);
            RuleFor(query => query.Filter.SnapshotKind).IsInEnum().When(query => query.Filter.SnapshotKind is not null);
            RuleFor(query => query.Filter.To).GreaterThanOrEqualTo(query => query.Filter.From).When(query => query.Filter.From is not null && query.Filter.To is not null);
        }
    }
}
