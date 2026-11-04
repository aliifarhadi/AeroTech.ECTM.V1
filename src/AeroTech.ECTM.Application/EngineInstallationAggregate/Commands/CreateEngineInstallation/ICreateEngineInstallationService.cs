using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation
{
    public interface ICreateEngineInstallationService
    {
        Task<BackofficeEngineInstallationDto> CreateAsync(ICreateEngineInstallationCommand command, CancellationToken cancellationToken = default);
    }
}
