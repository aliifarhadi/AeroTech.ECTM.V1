using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts
{
    public interface IConfigurationDocumentRepository
    {
        Task AddAsync(ConfigurationDocument document, CancellationToken cancellationToken = default);

        Task<ConfigurationDocument?> GetAsync(long id, CancellationToken cancellationToken = default);

        Task<ConfigurationDocument?> FindActiveAsync(ConfigurationKind kind, CancellationToken cancellationToken = default);

        Task<int> LastVersionNumberAsync(ConfigurationKind kind, CancellationToken cancellationToken = default);

        Task<IReadOnlyDictionary<ConfigurationKind, int>> CountByKindAsync(ConfigurationDocumentStatus status, CancellationToken cancellationToken = default);

        Task<PaginatedList<ConfigurationDocument>> ListAsync(ConfigurationKind kind, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
