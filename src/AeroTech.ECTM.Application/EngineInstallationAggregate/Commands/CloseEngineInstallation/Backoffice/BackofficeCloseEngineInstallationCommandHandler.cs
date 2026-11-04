using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation.Backoffice
{
    public sealed class BackofficeCloseEngineInstallationCommandHandler : IRequestHandler<BackofficeCloseEngineInstallationCommand, BackofficeEngineInstallationDto>
    {
        private readonly ICloseEngineInstallationService _service;

        public BackofficeCloseEngineInstallationCommandHandler(ICloseEngineInstallationService service) => _service = service;

        public Task<BackofficeEngineInstallationDto> Handle(BackofficeCloseEngineInstallationCommand command, CancellationToken cancellationToken)
            => _service.CloseAsync(command, cancellationToken);
    }
}
