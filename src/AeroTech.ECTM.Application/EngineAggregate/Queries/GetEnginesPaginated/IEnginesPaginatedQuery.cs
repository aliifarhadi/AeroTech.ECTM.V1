using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated
{
    public interface IEnginesPaginatedQuery
    {
        string? Search { get; }

        long? EngineModelId { get; }

        EngineStatus? Status { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
