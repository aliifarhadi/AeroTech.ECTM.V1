using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument
{
    public interface IRetireConfigurationDocumentService
    {
        Task<BackofficeConfigurationDocumentDetailDto> RetireAsync(IRetireConfigurationDocumentCommand command, CancellationToken cancellationToken = default);
    }
}
