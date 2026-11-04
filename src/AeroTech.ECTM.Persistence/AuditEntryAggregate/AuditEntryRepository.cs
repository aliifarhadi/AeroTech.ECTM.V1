using AeroTech.ECTM.Domain.AuditEntryAggregate;
using AeroTech.ECTM.Domain.AuditEntryAggregate.Arguments;
using AeroTech.ECTM.Domain.AuditEntryAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.AuditEntryAggregate
{
    public sealed class AuditEntryRepository : IAuditEntryRepository
    {
        private readonly EctmDbContext _dbContext;

        public AuditEntryRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public Task<PaginatedList<AuditEntry>> ListAsync(AuditEntryFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.AuditEntries.AsNoTracking();

            if (filter.EntityType is { } entityType)
                query = query.Where(entry => entry.EntityType == entityType);

            if (filter.EntityId is { } entityId)
                query = query.Where(entry => entry.EntityId == entityId);

            if (filter.ActorSubject is { } actorSubject)
                query = query.Where(entry => entry.ActorSubject == actorSubject);

            if (filter.From is { } from)
                query = query.Where(entry => entry.OccurredAt >= from);

            if (filter.To is { } to)
                query = query.Where(entry => entry.OccurredAt <= to);

            return query
                .OrderByDescending(entry => entry.OccurredAt)
                .ThenByDescending(entry => entry.Id)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
