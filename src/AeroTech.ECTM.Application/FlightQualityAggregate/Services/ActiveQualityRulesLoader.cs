using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Services
{
    public sealed class ActiveQualityRulesLoader
    {
        private readonly IConfigurationDocumentRepository _documents;
        private readonly ActiveCatalogLoader _activeCatalog;

        public ActiveQualityRulesLoader(IConfigurationDocumentRepository documents, ActiveCatalogLoader activeCatalog)
        {
            _documents = documents;
            _activeCatalog = activeCatalog;
        }

        public async Task<ActiveQualityRules?> FindAsync(CancellationToken cancellationToken = default)
        {
            var document = await _documents.FindActiveAsync(ConfigurationKind.QUALITY_RULES, cancellationToken);

            if (document is null)
                return null;

            var catalog = await _activeCatalog.LoadAsync(cancellationToken);
            var validation = QualityRulesValidator.Validate(document.Content, catalog);

            if (!validation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(document.Id, validation.Issues.Count);

            return new ActiveQualityRules(document, validation.Rules!);
        }

        public async Task<ActiveQualityRules> LoadAsync(CancellationToken cancellationToken = default)
            => await FindAsync(cancellationToken)
               ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.QUALITY_RULES);
    }
}
