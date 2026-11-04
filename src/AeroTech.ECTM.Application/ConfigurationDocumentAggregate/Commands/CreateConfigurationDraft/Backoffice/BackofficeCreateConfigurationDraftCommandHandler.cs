using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.CreateConfigurationDraft.Backoffice
{
    public sealed class BackofficeCreateConfigurationDraftCommandHandler : IRequestHandler<BackofficeCreateConfigurationDraftCommand, BackofficeConfigurationDocumentDetailDto>
    {
        private readonly ICreateConfigurationDraftService _service;

        public BackofficeCreateConfigurationDraftCommandHandler(ICreateConfigurationDraftService service) => _service = service;

        public Task<BackofficeConfigurationDocumentDetailDto> Handle(BackofficeCreateConfigurationDraftCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
