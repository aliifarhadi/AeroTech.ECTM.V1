using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AeroTech.ECTM.Domain.AuditEntryAggregate;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AeroTech.ECTM.Persistence.AuditEntryAggregate
{
    internal static class AuditEntryRecorder
    {
        private static readonly JsonSerializerOptions SnapshotOptions = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly HashSet<string> PropertiesLeftOut =
        [
            nameof(AggregateRoot<long>.RowVersion),
            nameof(Entity<long>.LastUpdateTime),
            nameof(Entity<long>.LastUpdatedBy)
        ];

        public static IReadOnlyList<AuditEntry> Record(ChangeTracker changeTracker, Actor actor, DateTimeOffset occurredAt, IIdGenerator idGenerator)
        {
            var correlationId = Activity.Current?.TraceId.ToHexString();
            var changedEntries = changeTracker.Entries<IEntity>().ToList();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in changedEntries)
            {
                if (entry.Entity is AuditEntry)
                    continue;

                if (ActionOf(entry.State) is not { } action)
                    continue;

                var auditEntry = AuditEntry.Record(
                    idGenerator.NewId(),
                    occurredAt,
                    actor.Subject,
                    actor.ActorId,
                    entry.Metadata.ClrType.Name,
                    EntityIdOf(entry),
                    action,
                    BeforeOf(entry, action),
                    AfterOf(entry, action),
                    correlationId);

                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }

        private static AuditAction? ActionOf(EntityState state)
            => state switch
            {
                EntityState.Added => AuditAction.CREATED,
                EntityState.Modified => AuditAction.UPDATED,
                EntityState.Deleted => AuditAction.DELETED,
                _ => null
            };

        private static long EntityIdOf(EntityEntry entry)
            => (long)entry.Property(nameof(Entity<long>.Id)).CurrentValue!;

        private static string? BeforeOf(EntityEntry entry, AuditAction action)
        {
            if (action == AuditAction.CREATED)
                return null;

            return SnapshotOf(entry, entry.OriginalValues, onlyModified: action == AuditAction.UPDATED);
        }

        private static string? AfterOf(EntityEntry entry, AuditAction action)
        {
            if (action == AuditAction.DELETED)
                return null;

            return SnapshotOf(entry, entry.CurrentValues, onlyModified: action == AuditAction.UPDATED);
        }

        private static string SnapshotOf(EntityEntry entry, PropertyValues values, bool onlyModified)
        {
            var snapshot = new Dictionary<string, object?>();

            foreach (var property in values.Properties)
            {
                if (PropertiesLeftOut.Contains(property.Name))
                    continue;

                var generatedByDatabase = property.ValueGenerated == ValueGenerated.OnAdd;

                if (generatedByDatabase)
                    continue;

                var unchanged = !entry.Property(property.Name).IsModified;

                if (onlyModified && unchanged)
                    continue;

                snapshot[property.Name] = values[property];
            }

            return JsonSerializer.Serialize(snapshot, SnapshotOptions);
        }
    }
}
