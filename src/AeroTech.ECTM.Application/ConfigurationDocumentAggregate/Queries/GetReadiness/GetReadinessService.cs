using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness
{
    public sealed class GetReadinessService : IGetReadinessService
    {
        private readonly IConfigurationDocumentRepository _documents;

        public GetReadinessService(IConfigurationDocumentRepository documents) => _documents = documents;

        public async Task<BackofficeReadinessDto> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var kinds = new List<BackofficeReadinessKindDto>();
            var missingForParser = new List<string>();

            foreach (var kind in Enum.GetValues<ConfigurationKind>())
            {
                var activeDocument = await _documents.FindActiveAsync(kind, cancellationToken);
                var requiredForParser = ConfigurationKinds.RequiredForParser.Contains(kind);

                if (requiredForParser && activeDocument is null)
                    missingForParser.Add(kind.ToString());

                kinds.Add(new BackofficeReadinessKindDto(
                    EnumValueDto.Of(kind),
                    ConfigurationKinds.DeliveryPhase[kind],
                    requiredForParser,
                    activeDocument?.Id,
                    activeDocument?.VersionNumber));
            }

            return new BackofficeReadinessDto(missingForParser.Count == 0, missingForParser, kinds);
        }
    }
}
