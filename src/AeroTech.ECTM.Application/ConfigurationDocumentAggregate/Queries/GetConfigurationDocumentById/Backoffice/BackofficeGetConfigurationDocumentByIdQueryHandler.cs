using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById.Backoffice
{
    public sealed class BackofficeGetConfigurationDocumentByIdQueryHandler : IRequestHandler<BackofficeGetConfigurationDocumentByIdQuery, BackofficeConfigurationDocumentDetailDto>
    {
        private readonly IGetConfigurationDocumentByIdService _service;

        public BackofficeGetConfigurationDocumentByIdQueryHandler(IGetConfigurationDocumentByIdService service) => _service = service;

        public Task<BackofficeConfigurationDocumentDetailDto> Handle(BackofficeGetConfigurationDocumentByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.DocumentId, cancellationToken);
    }
}
