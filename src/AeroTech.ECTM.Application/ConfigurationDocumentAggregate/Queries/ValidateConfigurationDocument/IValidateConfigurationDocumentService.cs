using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.ValidateConfigurationDocument
{
    public interface IValidateConfigurationDocumentService
    {
        Task<BackofficeValidationResultDto> ExecuteAsync(string kind, long documentId, CancellationToken cancellationToken = default);
    }
}
