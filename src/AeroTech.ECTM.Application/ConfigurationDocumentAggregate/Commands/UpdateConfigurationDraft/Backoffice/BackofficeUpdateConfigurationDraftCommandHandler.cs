using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.UpdateConfigurationDraft.Backoffice
{
    public sealed class BackofficeUpdateConfigurationDraftCommandHandler : IRequestHandler<BackofficeUpdateConfigurationDraftCommand, BackofficeConfigurationDocumentDetailDto>
    {
        private readonly IUpdateConfigurationDraftService _service;

        public BackofficeUpdateConfigurationDraftCommandHandler(IUpdateConfigurationDraftService service) => _service = service;

        public Task<BackofficeConfigurationDocumentDetailDto> Handle(BackofficeUpdateConfigurationDraftCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
