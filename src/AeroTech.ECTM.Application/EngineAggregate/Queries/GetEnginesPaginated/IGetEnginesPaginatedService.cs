using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated
{
    public interface IGetEnginesPaginatedService
    {
        Task<PaginatedList<BackofficeEngineDto>> ExecuteAsync(IEnginesPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
