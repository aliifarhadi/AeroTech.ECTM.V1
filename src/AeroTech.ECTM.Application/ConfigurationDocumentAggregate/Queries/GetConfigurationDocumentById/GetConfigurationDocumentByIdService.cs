using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById
{
    public sealed class GetConfigurationDocumentByIdService : IGetConfigurationDocumentByIdService
    {
        private readonly ConfigurationDocumentLoader _documents;

        public GetConfigurationDocumentByIdService(ConfigurationDocumentLoader documents) => _documents = documents;

        public async Task<BackofficeConfigurationDocumentDetailDto> ExecuteAsync(string kind, long documentId, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(kind, documentId, cancellationToken);

            return BackofficeConfigurationDocumentDetailDto.From(document);
        }
    }
}
