using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application._Shared.Paging
{
    public static class PaginatedListExtensions
    {
        public static PaginatedList<TResult> Map<TItem, TResult>(this PaginatedList<TItem> page, Func<TItem, TResult> map)
            => PaginatedList<TResult>.Create(page.Results.Select(map).ToList(), page.PageNumber, page.PageSize, page.TotalCount);
    }
}
