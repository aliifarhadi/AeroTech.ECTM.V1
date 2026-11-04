using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated
{
    public interface IGetConfigurationDocumentsPaginatedService
    {
        Task<PaginatedList<BackofficeConfigurationDocumentDto>> ExecuteAsync(IConfigurationDocumentsPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
