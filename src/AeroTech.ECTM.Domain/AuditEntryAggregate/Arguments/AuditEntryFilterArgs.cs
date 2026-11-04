namespace AeroTech.ECTM.Domain.AuditEntryAggregate.Arguments
{
    public sealed record AuditEntryFilterArgs(
        string? EntityType,
        long? EntityId,
        string? ActorSubject,
        DateTimeOffset? From,
        DateTimeOffset? To);
}
