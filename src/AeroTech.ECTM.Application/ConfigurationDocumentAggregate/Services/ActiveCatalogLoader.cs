using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services
{
    public sealed class ActiveCatalogLoader
    {
        private readonly IConfigurationDocumentRepository _documents;

        public ActiveCatalogLoader(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<ValidatedCatalog?> LoadAsync(CancellationToken cancellationToken = default)
        {
            var activeCatalog = await _documents.FindActiveAsync(ConfigurationKind.PARAMETER_CATALOG, cancellationToken);

            if (activeCatalog is null)
                return null;

            return ParameterCatalogValidator.Validate(activeCatalog.Content).Catalog;
        }
    }
}
