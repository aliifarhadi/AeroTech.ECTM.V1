using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence._Shared
{
    internal static class PagingExtensions
    {
        public static async Task<PaginatedList<TItem>> ToPaginatedListAsync<TItem>(
            this IOrderedQueryable<TItem> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var totalCount = await query.LongCountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return PaginatedList<TItem>.Create(items, pageNumber, pageSize, totalCount);
        }
    }
}
