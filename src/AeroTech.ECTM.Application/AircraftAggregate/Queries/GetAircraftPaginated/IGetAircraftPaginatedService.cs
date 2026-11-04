using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated
{
    public interface IGetAircraftPaginatedService
    {
        Task<PaginatedList<BackofficeAircraftDto>> ExecuteAsync(IAircraftPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
