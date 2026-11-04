using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Entities;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Dto
{
    public sealed record BackofficeRegistryChangeDto(
        EnumValueDto ChangeType,
        string Reason,
        DateTimeOffset ChangedUtc,
        long RegistryVersion)
    {
        public static BackofficeRegistryChangeDto From(RegistryChange change)
            => new(EnumValueDto.Of(change.ChangeType), change.Reason, change.ChangedAt, change.RegistryVersion);
    }
}
