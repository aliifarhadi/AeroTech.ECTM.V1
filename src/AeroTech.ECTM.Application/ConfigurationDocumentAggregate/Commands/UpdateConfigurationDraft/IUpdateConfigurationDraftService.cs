using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft
{
    public interface IUpdateConfigurationDraftService
    {
        Task<BackofficeConfigurationDocumentDetailDto> UpdateAsync(IUpdateConfigurationDraftCommand command, CancellationToken cancellationToken = default);
    }
}
