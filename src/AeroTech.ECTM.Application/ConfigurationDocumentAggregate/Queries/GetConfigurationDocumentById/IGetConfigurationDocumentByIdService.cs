using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentById
{
    public interface IGetConfigurationDocumentByIdService
    {
        Task<BackofficeConfigurationDocumentDetailDto> ExecuteAsync(string kind, long documentId, CancellationToken cancellationToken = default);
    }
}
