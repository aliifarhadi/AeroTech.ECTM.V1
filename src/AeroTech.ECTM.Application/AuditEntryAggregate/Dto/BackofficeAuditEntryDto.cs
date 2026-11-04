using System.Text.Json;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application._Shared.Json;
using AeroTech.ECTM.Domain.AuditEntryAggregate;

namespace AeroTech.ECTM.Application.AuditEntryAggregate.Dto
{
    public sealed record BackofficeAuditEntryDto(
        long Id,
        DateTimeOffset OccurredAt,
        string? ActorSubject,
        long? ActorId,
        string EntityType,
        long EntityId,
        EnumValueDto Action,
        JsonElement? Before,
        JsonElement? After,
        string? CorrelationId)
    {
        public static BackofficeAuditEntryDto From(AuditEntry entry)
            => new(
                entry.Id,
                entry.OccurredAt,
                entry.ActorSubject,
                entry.ActorId,
                entry.EntityType,
                entry.EntityId,
                EnumValueDto.Of(entry.Action),
                StoredJson.ParseNullable(entry.Before),
                StoredJson.ParseNullable(entry.After),
                entry.CorrelationId);
    }
}
