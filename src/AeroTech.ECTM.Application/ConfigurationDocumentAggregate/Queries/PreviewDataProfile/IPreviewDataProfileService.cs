using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.PreviewDataProfile
{
    public interface IPreviewDataProfileService
    {
        Task<BackofficeProfilePreviewDto> ExecuteAsync(string kind, long documentId, string fileName, byte[] content, CancellationToken cancellationToken = default);
    }
}
