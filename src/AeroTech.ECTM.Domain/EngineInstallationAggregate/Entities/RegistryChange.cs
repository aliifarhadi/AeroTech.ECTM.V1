using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.EngineInstallationAggregate.Entities
{
    public sealed class RegistryChange : Entity<long>
    {
        private RegistryChange()
        {
        }

        internal RegistryChange(long id, long installationId, RegistryChangeType changeType, string reason, DateTimeOffset changedAt)
        {
            Id = id;
            InstallationId = installationId;
            ChangeType = changeType;
            Reason = reason;
            ChangedAt = changedAt;
        }

        public long RegistryVersion { get; private set; }

        public long InstallationId { get; private set; }

        public RegistryChangeType ChangeType { get; private set; }

        public string Reason { get; private set; } = default!;

        public DateTimeOffset ChangedAt { get; private set; }
    }
}
