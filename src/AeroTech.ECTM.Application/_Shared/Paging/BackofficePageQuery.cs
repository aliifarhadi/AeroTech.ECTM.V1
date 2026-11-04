using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application._Shared.Paging
{
    public abstract class BackofficePageQuery : PaginationQuery
    {
        protected BackofficePageQuery() => PageSize = PageLimits.DefaultPageSize;
    }
}
