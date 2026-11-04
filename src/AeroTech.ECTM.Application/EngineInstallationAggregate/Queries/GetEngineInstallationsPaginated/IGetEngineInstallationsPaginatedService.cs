using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated
{
    public interface IGetEngineInstallationsPaginatedService
    {
        Task<PaginatedList<BackofficeEngineInstallationDto>> ExecuteAsync(IEngineInstallationsPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
