using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff
{
    public interface IGetConfigurationDocumentDiffService
    {
        Task<BackofficeConfigurationDiffDto> ExecuteAsync(string kind, long fromDocumentId, long toDocumentId, CancellationToken cancellationToken = default);
    }
}
