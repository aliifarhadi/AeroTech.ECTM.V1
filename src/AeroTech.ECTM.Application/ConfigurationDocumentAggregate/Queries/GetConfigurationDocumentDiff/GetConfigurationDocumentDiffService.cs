using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff
{
    public sealed class GetConfigurationDocumentDiffService : IGetConfigurationDocumentDiffService
    {
        private readonly ConfigurationDocumentLoader _documents;

        public GetConfigurationDocumentDiffService(ConfigurationDocumentLoader documents) => _documents = documents;

        public async Task<BackofficeConfigurationDiffDto> ExecuteAsync(string kind, long fromDocumentId, long toDocumentId, CancellationToken cancellationToken = default)
        {
            var from = await _documents.LoadAsync(kind, fromDocumentId, cancellationToken);
            var to = await _documents.LoadAsync(kind, toDocumentId, cancellationToken);
            var changes = JsonDiff.Compare(from.Content, to.Content);

            return new BackofficeConfigurationDiffDto(
                BackofficeConfigurationDocumentDto.From(from),
                BackofficeConfigurationDocumentDto.From(to),
                changes.Select(BackofficeJsonChangeDto.From).ToList());
        }
    }
}
