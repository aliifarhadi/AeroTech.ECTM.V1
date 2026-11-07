using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline.Backoffice
{
    public sealed class BackofficePublishBaselineCommandHandler : IRequestHandler<BackofficePublishBaselineCommand, BackofficeBaselinePublicationDto>
    {
        private readonly IPublishBaselineService _service;

        public BackofficePublishBaselineCommandHandler(IPublishBaselineService service) => _service = service;

        public Task<BackofficeBaselinePublicationDto> Handle(BackofficePublishBaselineCommand command, CancellationToken cancellationToken)
            => _service.PublishAsync(command, cancellationToken);
    }
}
