using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentsPaginated.Backoffice
{
    public sealed class BackofficeGetConfigurationDocumentsPaginatedQueryHandler : IRequestHandler<BackofficeGetConfigurationDocumentsPaginatedQuery, PaginatedList<BackofficeConfigurationDocumentDto>>
    {
        private readonly IGetConfigurationDocumentsPaginatedService _service;

        public BackofficeGetConfigurationDocumentsPaginatedQueryHandler(IGetConfigurationDocumentsPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeConfigurationDocumentDto>> Handle(BackofficeGetConfigurationDocumentsPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
