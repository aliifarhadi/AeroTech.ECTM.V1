using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.RetireConfigurationDocument.Backoffice
{
    public sealed class BackofficeRetireConfigurationDocumentCommandHandler : IRequestHandler<BackofficeRetireConfigurationDocumentCommand, BackofficeConfigurationDocumentDetailDto>
    {
        private readonly IRetireConfigurationDocumentService _service;

        public BackofficeRetireConfigurationDocumentCommandHandler(IRetireConfigurationDocumentService service) => _service = service;

        public Task<BackofficeConfigurationDocumentDetailDto> Handle(BackofficeRetireConfigurationDocumentCommand command, CancellationToken cancellationToken)
            => _service.RetireAsync(command, cancellationToken);
    }
}
