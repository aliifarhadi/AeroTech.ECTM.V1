using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated
{
    public sealed class GetConfigurationDocumentsPaginatedService : IGetConfigurationDocumentsPaginatedService
    {
        private readonly IConfigurationDocumentRepository _documents;

        public GetConfigurationDocumentsPaginatedService(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<PaginatedList<BackofficeConfigurationDocumentDto>> ExecuteAsync(IConfigurationDocumentsPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var kind = ConfigurationKinds.Parse(query.Kind);
            var page = await _documents.ListAsync(kind, query.PageNumber, query.PageSize, cancellationToken);

            return page.Map(BackofficeConfigurationDocumentDto.From);
        }
    }
}
