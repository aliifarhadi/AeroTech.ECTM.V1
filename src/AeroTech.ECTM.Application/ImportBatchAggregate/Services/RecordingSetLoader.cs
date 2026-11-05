using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class RecordingSetLoader
    {
        private readonly IConfigurationDocumentRepository _documents;

        public RecordingSetLoader(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<RecordingSet> LoadActiveAsync(CancellationToken cancellationToken = default)
        {
            var profileDocument = await _documents.FindActiveAsync(ConfigurationKind.DATA_PROFILE, cancellationToken)
                                  ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.DATA_PROFILE);
            var catalogDocument = await _documents.FindActiveAsync(ConfigurationKind.PARAMETER_CATALOG, cancellationToken)
                                  ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.PARAMETER_CATALOG);

            return SetOf(profileDocument, catalogDocument);
        }

        public async Task<RecordingSet> LoadPinnedAsync(ConfigurationVersions versions, CancellationToken cancellationToken = default)
        {
            var profileDocument = await _documents.GetAsync(versions.DataProfileDocumentId, cancellationToken)
                                  ?? throw ExceptionFactory.ConfigurationDocumentNotFound(versions.DataProfileDocumentId);
            var catalogDocument = await _documents.GetAsync(versions.ParameterCatalogDocumentId, cancellationToken)
                                  ?? throw ExceptionFactory.ConfigurationDocumentNotFound(versions.ParameterCatalogDocumentId);

            return SetOf(profileDocument, catalogDocument);
        }

        private static RecordingSet SetOf(ConfigurationDocument profileDocument, ConfigurationDocument catalogDocument)
        {
            var catalog = ParameterCatalogValidator.Validate(catalogDocument.Content).Catalog
                          ?? throw ExceptionFactory.ConfigurationDocumentIsInvalid(catalogDocument.Id, 1);
            var profileValidation = DataProfileValidator.Validate(profileDocument.Content, catalog, forPublish: false);

            if (!profileValidation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(profileDocument.Id, profileValidation.Issues.Count);

            var versions = new ConfigurationVersions(profileDocument.Id, profileDocument.VersionNumber, catalogDocument.Id, catalogDocument.VersionNumber);

            return new RecordingSet(versions, profileValidation.Profile!, catalog);
        }
    }
}
