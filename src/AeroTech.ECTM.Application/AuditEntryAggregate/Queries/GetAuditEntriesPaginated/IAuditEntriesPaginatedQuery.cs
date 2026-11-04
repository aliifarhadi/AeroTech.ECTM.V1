namespace AeroTech.ECTM.Application.AuditEntryAggregate.Queries.GetAuditEntriesPaginated
{
    public interface IAuditEntriesPaginatedQuery
    {
        string? EntityType { get; }

        long? EntityId { get; }

        string? Actor { get; }

        DateTimeOffset? From { get; }

        DateTimeOffset? To { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
