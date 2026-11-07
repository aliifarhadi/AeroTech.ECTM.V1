using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CreateBaseline.Backoffice
{
    public sealed class BackofficeCreateBaselineCommandHandler : IRequestHandler<BackofficeCreateBaselineCommand, BackofficeBaselineDto>
    {
        private readonly ICreateBaselineService _service;

        public BackofficeCreateBaselineCommandHandler(ICreateBaselineService service) => _service = service;

        public Task<BackofficeBaselineDto> Handle(BackofficeCreateBaselineCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
