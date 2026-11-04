using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation.Backoffice
{
    public sealed record BackofficeCreateEngineInstallationCommand(
        long EngineId,
        long AircraftId,
        int Position,
        DateTimeOffset InstalledUtc,
        string Reason,
        string? Evidence) : IRequest<BackofficeEngineInstallationDto>, ICreateEngineInstallationCommand;
}
