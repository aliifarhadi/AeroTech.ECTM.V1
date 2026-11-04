using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.AuditEntryAggregate
{
    public sealed class AuditEntry : AggregateRoot<long>
    {
        private AuditEntry()
        {
        }

        private AuditEntry(
            long id,
            DateTimeOffset occurredAt,
            string? actorSubject,
            long? actorId,
            string entityType,
            long entityId,
            AuditAction action,
            string? before,
            string? after,
            string? correlationId)
        {
            Id = id;
            OccurredAt = occurredAt;
            ActorSubject = actorSubject;
            ActorId = actorId;
            EntityType = entityType;
            EntityId = entityId;
            Action = action;
            Before = before;
            After = after;
            CorrelationId = correlationId;
        }

        public DateTimeOffset OccurredAt { get; private set; }

        public string? ActorSubject { get; private set; }

        public long? ActorId { get; private set; }

        public string EntityType { get; private set; } = default!;

        public long EntityId { get; private set; }

        public AuditAction Action { get; private set; }

        public string? Before { get; private set; }

        public string? After { get; private set; }

        public string? CorrelationId { get; private set; }

        public static AuditEntry Record(
            long id,
            DateTimeOffset occurredAt,
            string? actorSubject,
            long? actorId,
            string entityType,
            long entityId,
            AuditAction action,
            string? before,
            string? after,
            string? correlationId)
            => new(
                id,
                occurredAt.ToUniversalTime(),
                actorSubject,
                actorId,
                entityType,
                entityId,
                action,
                before,
                after,
                correlationId);
    }
}
