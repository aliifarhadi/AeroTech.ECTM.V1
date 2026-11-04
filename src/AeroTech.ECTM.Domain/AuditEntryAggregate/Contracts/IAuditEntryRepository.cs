using AeroTech.ECTM.Domain.AuditEntryAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Domain.AuditEntryAggregate.Contracts
{
    public interface IAuditEntryRepository
    {
        Task<PaginatedList<AuditEntry>> ListAsync(AuditEntryFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
