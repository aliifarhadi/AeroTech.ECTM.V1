using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated
{
    public interface IGetFlightsPaginatedService
    {
        Task<PaginatedList<BackofficeFlightSummaryDto>> ExecuteAsync(IFlightsPaginatedQuery query, CancellationToken cancellationToken = default);
    }
}
