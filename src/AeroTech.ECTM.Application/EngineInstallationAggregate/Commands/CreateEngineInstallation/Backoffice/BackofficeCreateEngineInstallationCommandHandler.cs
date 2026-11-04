using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation.Backoffice
{
    public sealed class BackofficeCreateEngineInstallationCommandHandler : IRequestHandler<BackofficeCreateEngineInstallationCommand, BackofficeEngineInstallationDto>
    {
        private readonly ICreateEngineInstallationService _service;

        public BackofficeCreateEngineInstallationCommandHandler(ICreateEngineInstallationService service) => _service = service;

        public Task<BackofficeEngineInstallationDto> Handle(BackofficeCreateEngineInstallationCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
