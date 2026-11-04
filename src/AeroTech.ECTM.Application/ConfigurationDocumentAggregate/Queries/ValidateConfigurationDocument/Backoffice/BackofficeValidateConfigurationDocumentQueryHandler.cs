using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument.Backoffice
{
    public sealed class BackofficeValidateConfigurationDocumentQueryHandler : IRequestHandler<BackofficeValidateConfigurationDocumentQuery, BackofficeValidationResultDto>
    {
        private readonly IValidateConfigurationDocumentService _service;

        public BackofficeValidateConfigurationDocumentQueryHandler(IValidateConfigurationDocumentService service) => _service = service;

        public Task<BackofficeValidationResultDto> Handle(BackofficeValidateConfigurationDocumentQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.DocumentId, cancellationToken);
    }
}
