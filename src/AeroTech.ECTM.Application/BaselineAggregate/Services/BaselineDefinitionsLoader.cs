using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Services
{
    public sealed class BaselineDefinitionsLoader
    {
        private readonly IConfigurationDocumentRepository _documents;

        public BaselineDefinitionsLoader(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<BaselineDefinitions> LoadActiveAsync(CancellationToken cancellationToken = default)
        {
            var catalogDocument = await ActiveAsync(ConfigurationKind.PARAMETER_CATALOG, cancellationToken);
            var catalog = ParameterCatalogValidator.Validate(catalogDocument.Content).Catalog
                          ?? throw ExceptionFactory.ConfigurationDocumentIsInvalid(catalogDocument.Id, 1);

            var snapshotDocument = await ActiveAsync(ConfigurationKind.SNAPSHOT_RULES, cancellationToken);
            var snapshot = SnapshotRulesValidator.Validate(snapshotDocument.Content, catalog);

            if (!snapshot.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(snapshotDocument.Id, snapshot.Issues.Count);

            return new BaselineDefinitions(snapshotDocument, snapshot.Rules!, catalogDocument, catalog);
        }

        private async Task<ConfigurationDocument> ActiveAsync(ConfigurationKind kind, CancellationToken cancellationToken)
            => await _documents.FindActiveAsync(kind, cancellationToken)
               ?? throw ExceptionFactory.ActiveSetIncomplete(kind);
    }
}
