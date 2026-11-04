using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument
{
    public interface IPublishConfigurationDocumentService
    {
        Task<BackofficeConfigurationDocumentDetailDto> PublishAsync(IPublishConfigurationDocumentCommand command, CancellationToken cancellationToken = default);
    }
}
