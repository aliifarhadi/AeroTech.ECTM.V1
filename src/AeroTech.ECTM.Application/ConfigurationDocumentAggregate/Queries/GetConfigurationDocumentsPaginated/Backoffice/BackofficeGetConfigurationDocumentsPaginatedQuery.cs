using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated.Backoffice
{
    public sealed class BackofficeGetConfigurationDocumentsPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeConfigurationDocumentDto>>, IConfigurationDocumentsPaginatedQuery
    {
        public string? Kind { get; set; }
    }
}
