using System.Security.Cryptography;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile
{
    public sealed class PreviewDataProfileService : IPreviewDataProfileService
    {
        public const int MaximumFileBytes = 50 * 1024 * 1024;

        private const string CatalogDryRunPhase = "P03 (dry-run on imported flights)";

        private readonly ConfigurationDocumentLoader _documents;
        private readonly ActiveCatalogLoader _activeCatalog;

        public PreviewDataProfileService(ConfigurationDocumentLoader documents, ActiveCatalogLoader activeCatalog)
        {
            _documents = documents;
            _activeCatalog = activeCatalog;
        }

        public async Task<BackofficeProfilePreviewDto> ExecuteAsync(string kind, long documentId, string fileName, byte[] content, CancellationToken cancellationToken = default)
        {
            var document = await _documents.LoadAsync(kind, documentId, cancellationToken);

            if (document.Kind != ConfigurationKind.DATA_PROFILE)
                throw ExceptionFactory.DryRunNotAvailable(document.Kind, DryRunPhaseOf(document.Kind));

            if (content.Length > MaximumFileBytes)
                throw ExceptionFactory.SourceFileIsTooLarge(content.Length, MaximumFileBytes);

            var catalog = await _activeCatalog.LoadAsync(cancellationToken)
                          ?? throw ExceptionFactory.ActiveSetIncomplete(ConfigurationKind.PARAMETER_CATALOG);

            var validation = DataProfileValidator.Validate(document.Content, catalog, forPublish: false);

            if (!validation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(document.Id, validation.Issues.Count);

            var profile = validation.Profile!;
            var rows = CsvRows.Read(content, profile.Delimiter![0]);
            var preview = DataProfilePreviewer.Preview(profile, catalog, rows);
            var fileSha256 = Convert.ToHexStringLower(SHA256.HashData(content));

            return BackofficeProfilePreviewDto.From(document.Id, fileName, fileSha256, preview);
        }

        private static string DryRunPhaseOf(ConfigurationKind kind)
        {
            if (kind == ConfigurationKind.PARAMETER_CATALOG)
                return CatalogDryRunPhase;

            return ConfigurationKinds.DeliveryPhase[kind];
        }
    }
}
