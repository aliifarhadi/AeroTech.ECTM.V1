using FluentValidation;

namespace AeroTech.ECTM.Application._Shared.Paging
{
    public sealed class PageQueryValidator<TQuery> : AbstractValidator<TQuery>
        where TQuery : BackofficePageQuery
    {
        public PageQueryValidator()
        {
            RuleFor(query => query.PageNumber).InclusiveBetween(1, PageLimits.MaxPageNumber);
            RuleFor(query => query.PageSize).InclusiveBetween(1, PageLimits.MaxPageSize);
        }
    }
}
