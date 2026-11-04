using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ConfigurationDocumentLoader
    {
        private readonly IConfigurationDocumentRepository _documents;

        public ConfigurationDocumentLoader(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<ConfigurationDocument> LoadAsync(string? kindCode, long documentId, CancellationToken cancellationToken = default)
        {
            var kind = ConfigurationKinds.Parse(kindCode);
            var document = await _documents.GetAsync(documentId, cancellationToken);

            if (document is null)
                throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);

            if (document.Kind != kind)
                throw ExceptionFactory.ConfigurationDocumentNotFound(documentId);

            return document;
        }
    }
}
