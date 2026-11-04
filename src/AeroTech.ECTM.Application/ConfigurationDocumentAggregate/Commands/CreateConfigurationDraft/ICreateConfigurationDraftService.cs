using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft
{
    public interface ICreateConfigurationDraftService
    {
        Task<BackofficeConfigurationDocumentDetailDto> CreateAsync(ICreateConfigurationDraftCommand command, CancellationToken cancellationToken = default);
    }
}
