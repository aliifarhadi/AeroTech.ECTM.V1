using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation.Backoffice
{
    public sealed record BackofficeCloseEngineInstallationCommand(
        long InstallationId,
        DateTimeOffset RemovedUtc,
        string Reason) : IRequest<BackofficeEngineInstallationDto>, ICloseEngineInstallationCommand;
}
