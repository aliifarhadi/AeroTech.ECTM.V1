using AeroTech.ECTM.Domain.EngineInstallationAggregate;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Dto
{
    public sealed record BackofficeEngineInstallationDto(
        long Id,
        long EngineId,
        string EngineSerialNumber,
        long AircraftId,
        string AircraftRegistration,
        int Position,
        DateTimeOffset InstalledUtc,
        DateTimeOffset? RemovedUtc,
        string Reason,
        string? Evidence,
        string? RemovalReason,
        IReadOnlyList<BackofficeRegistryChangeDto> Changes)
    {
        public static BackofficeEngineInstallationDto From(EngineInstallation installation, string engineSerialNumber, string aircraftRegistration)
            => new(
                installation.Id,
                installation.EngineId,
                engineSerialNumber,
                installation.AircraftId,
                aircraftRegistration,
                installation.Position,
                installation.InstalledAt,
                installation.RemovedAt,
                installation.Reason,
                installation.Evidence,
                installation.RemovalReason,
                installation.Changes
                    .OrderBy(change => change.ChangedAt)
                    .ThenBy(change => change.RegistryVersion)
                    .Select(BackofficeRegistryChangeDto.From)
                    .ToList());
    }
}
