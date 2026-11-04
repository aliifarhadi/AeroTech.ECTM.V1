using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.EngineInstallationAggregate
{
    public sealed class EngineInstallation : AggregateRoot<long>
    {
        private readonly List<RegistryChange> _changes = new();

        private EngineInstallation()
        {
        }

        private EngineInstallation(long id, long engineId, long aircraftId, byte position, DateTimeOffset installedAt, string reason, string? evidence)
        {
            Id = id;
            EngineId = engineId;
            AircraftId = aircraftId;
            Position = position;
            InstalledAt = installedAt;
            Reason = reason;
            Evidence = evidence;
        }

        public long EngineId { get; private set; }

        public long AircraftId { get; private set; }

        public byte Position { get; private set; }

        public DateTimeOffset InstalledAt { get; private set; }

        public DateTimeOffset? RemovedAt { get; private set; }

        public string Reason { get; private set; } = default!;

        public string? Evidence { get; private set; }

        public string? RemovalReason { get; private set; }

        public bool IsOpen => RemovedAt is null;

        public IReadOnlyCollection<RegistryChange> Changes => _changes.AsReadOnly();

        public static EngineInstallation Record(
            long id,
            long engineId,
            long aircraftId,
            int position,
            DateTimeOffset installedAt,
            string reason,
            string? evidence,
            IIdGenerator idGenerator,
            DateTimeOffset now)
        {
            var validPosition = EnginePositions.EnsureValid(position);
            var installReason = RequiredReason(reason);

            var installation = new EngineInstallation(
                id,
                engineId,
                aircraftId,
                validPosition,
                installedAt.ToUniversalTime(),
                installReason,
                OptionalText.Normalize(evidence));

            installation.RecordChange(RegistryChangeType.INSTALLED, installReason, idGenerator, now);

            return installation;
        }

        public EngineInstallation? FindOverlapIn(IEnumerable<EngineInstallation> others)
        {
            foreach (var other in others)
            {
                if (other.Id == Id)
                    continue;

                if (!SharesResourceWith(other))
                    continue;

                if (Overlaps(other))
                    return other;
            }

            return null;
        }

        public void Close(DateTimeOffset removedAt, string reason, IIdGenerator idGenerator, DateTimeOffset now)
        {
            if (!IsOpen)
                throw ExceptionFactory.InstallationAlreadyClosed(Id);

            var removalTime = removedAt.ToUniversalTime();

            if (removalTime <= InstalledAt)
                throw ExceptionFactory.InstallationIntervalIsInvalid();

            var removalReason = RequiredReason(reason);

            RemovedAt = removalTime;
            RemovalReason = removalReason;

            RecordChange(RegistryChangeType.REMOVED, removalReason, idGenerator, now);
        }

        private bool SharesResourceWith(EngineInstallation other)
        {
            if (other.EngineId == EngineId)
                return true;

            return other.AircraftId == AircraftId && other.Position == Position;
        }

        private bool Overlaps(EngineInstallation other)
        {
            var thisEnd = RemovedAt ?? DateTimeOffset.MaxValue;
            var otherEnd = other.RemovedAt ?? DateTimeOffset.MaxValue;

            var startsBeforeOtherEnds = InstalledAt < otherEnd;
            var otherStartsBeforeThisEnds = other.InstalledAt < thisEnd;

            return startsBeforeOtherEnds && otherStartsBeforeThisEnds;
        }

        private void RecordChange(RegistryChangeType changeType, string reason, IIdGenerator idGenerator, DateTimeOffset now)
            => _changes.Add(new RegistryChange(idGenerator.NewId(), Id, changeType, reason, now));

        private static string RequiredReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw ExceptionFactory.RegistryChangeReasonIsRequired();

            return reason.Trim();
        }
    }
}
