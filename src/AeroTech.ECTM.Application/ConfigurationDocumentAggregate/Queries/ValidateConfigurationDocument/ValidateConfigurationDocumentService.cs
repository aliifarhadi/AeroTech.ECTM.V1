using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument
{
    public sealed class ValidateConfigurationDocumentService : IValidateConfigurationDocumentService
    {
        private readonly ConfigurationDocumentLoader _documents;
        private readonly ConfigurationContentValidators _validators;

        public ValidateConfigurationDocumentService(ConfigurationDocumentLoader documents, ConfigurationContentValidators validators)
        {
            _documents = documents;
            _validators = validators;
        }

        public async Task<BackofficeValidationResultDto> ExecuteAsync(string kind, long documentId, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(kind, documentId, cancellationToken);
            var validator = _validators.For(document.Kind);
            var issues = await validator.ValidateAsync(document.Content, forPublish: true, cancellationToken);

            return BackofficeValidationResultDto.From(issues);
        }
    }
}
