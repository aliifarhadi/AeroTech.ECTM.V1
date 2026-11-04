using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Commands.PublishConfigurationDocument.Backoffice
{
    public sealed class BackofficePublishConfigurationDocumentCommandHandler : IRequestHandler<BackofficePublishConfigurationDocumentCommand, BackofficeConfigurationDocumentDetailDto>
    {
        private readonly IPublishConfigurationDocumentService _service;

        public BackofficePublishConfigurationDocumentCommandHandler(IPublishConfigurationDocumentService service) => _service = service;

        public Task<BackofficeConfigurationDocumentDetailDto> Handle(BackofficePublishConfigurationDocumentCommand command, CancellationToken cancellationToken)
            => _service.PublishAsync(command, cancellationToken);
    }
}
