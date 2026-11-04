using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds
{
    public sealed class GetConfigurationKindsService : IGetConfigurationKindsService
    {
        private readonly IConfigurationDocumentRepository _documents;
        private readonly ConfigurationContentValidators _validators;

        public GetConfigurationKindsService(IConfigurationDocumentRepository documents, ConfigurationContentValidators validators)
        {
            _documents = documents;
            _validators = validators;
        }

        public async Task<IReadOnlyList<BackofficeConfigurationKindDto>> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var draftCounts = await _documents.CountByKindAsync(ConfigurationDocumentStatus.DRAFT, cancellationToken);
            var publishedCounts = await _documents.CountByKindAsync(ConfigurationDocumentStatus.PUBLISHED, cancellationToken);
            var kinds = new List<BackofficeConfigurationKindDto>();

            foreach (var kind in Enum.GetValues<ConfigurationKind>())
            {
                var activeDocument = await _documents.FindActiveAsync(kind, cancellationToken);
                var draftCount = draftCounts.GetValueOrDefault(kind);
                var publishedCount = publishedCounts.GetValueOrDefault(kind);
                var deliveryPhase = ConfigurationKinds.DeliveryPhase[kind];
                var authorable = _validators.IsAuthorable(kind);

                kinds.Add(BackofficeConfigurationKindDto.From(kind, deliveryPhase, authorable, activeDocument, draftCount, publishedCount));
            }

            return kinds;
        }
    }
}
