using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated
{
    public interface IGetEngineModelsPaginatedService
    {
        Task<PaginatedList<BackofficeEngineModelDto>> ExecuteAsync(IEngineModelsPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
