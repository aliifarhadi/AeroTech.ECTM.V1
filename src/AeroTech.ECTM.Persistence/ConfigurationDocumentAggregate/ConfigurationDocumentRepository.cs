using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Persistence._Shared;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Persistence.ConfigurationDocumentAggregate
{
    public sealed class ConfigurationDocumentRepository : IConfigurationDocumentRepository
    {
        private readonly EctmDbContext _dbContext;

        public ConfigurationDocumentRepository(EctmDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(ConfigurationDocument document, CancellationToken cancellationToken = default)
            => await _dbContext.ConfigurationDocuments.AddAsync(document, cancellationToken);

        public Task<ConfigurationDocument?> GetAsync(long id, CancellationToken cancellationToken = default)
            => _dbContext.ConfigurationDocuments.FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        public Task<ConfigurationDocument?> FindActiveAsync(ConfigurationKind kind, CancellationToken cancellationToken = default)
            => _dbContext.ConfigurationDocuments
                .AsNoTracking()
                .Where(document => document.Kind == kind)
                .Where(document => document.Status == ConfigurationDocumentStatus.PUBLISHED)
                .OrderByDescending(document => document.VersionNumber)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IReadOnlyList<ConfigurationDocument>> ListPublishedBetweenAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
            => await _dbContext.ConfigurationDocuments
                .AsNoTracking()
                .Where(document => document.PublishedAt >= from && document.PublishedAt < to)
                .OrderBy(document => document.PublishedAt)
                .ToListAsync(cancellationToken);

        public async Task<int> LastVersionNumberAsync(ConfigurationKind kind, CancellationToken cancellationToken = default)
        {
            var lastVersion = await _dbContext.ConfigurationDocuments
                .Where(document => document.Kind == kind)
                .MaxAsync(document => (int?)document.VersionNumber, cancellationToken);

            return lastVersion ?? 0;
        }

        public async Task<IReadOnlyDictionary<ConfigurationKind, int>> CountByKindAsync(ConfigurationDocumentStatus status, CancellationToken cancellationToken = default)
        {
            var counts = await _dbContext.ConfigurationDocuments
                .AsNoTracking()
                .Where(document => document.Status == status)
                .GroupBy(document => document.Kind)
                .Select(group => new { Kind = group.Key, Count = group.Count() })
                .ToListAsync(cancellationToken);

            return counts.ToDictionary(count => count.Kind, count => count.Count);
        }

        public Task<PaginatedList<ConfigurationDocument>> ListAsync(ConfigurationKind kind, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
            => _dbContext.ConfigurationDocuments
                .AsNoTracking()
                .Where(document => document.Kind == kind)
                .OrderByDescending(document => document.VersionNumber)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
