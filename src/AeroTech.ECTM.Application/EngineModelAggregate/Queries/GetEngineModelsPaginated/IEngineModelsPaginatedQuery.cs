namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated
{
    public interface IEngineModelsPaginatedQuery
    {
        string? Search { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
