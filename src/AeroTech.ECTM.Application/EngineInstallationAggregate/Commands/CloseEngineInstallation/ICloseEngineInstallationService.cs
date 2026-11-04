using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation
{
    public interface ICloseEngineInstallationService
    {
        Task<BackofficeEngineInstallationDto> CloseAsync(ICloseEngineInstallationCommand command, CancellationToken cancellationToken = default);
    }
}
