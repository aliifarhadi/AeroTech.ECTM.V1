using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById
{
    public sealed class GetConfigurationDocumentByIdService : IGetConfigurationDocumentByIdService
    {
        private readonly IConfigurationDocumentRepository _documents;

        public GetConfigurationDocumentByIdService(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<BackofficeConfigurationDocumentDetailDto> ExecuteAsync(string kind, long documentId, CancellationToken cancellationToken = default)
        {
            var configurationKind = ConfigurationKinds.Parse(kind);
            var document = await _documents.GetAsync(documentId, cancellationToken);

            if (document is null)
                throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);

            if (document.Kind != configurationKind)
                throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);

            return BackofficeConfigurationDocumentDetailDto.From(document);
        }
    }
}
