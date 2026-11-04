using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated
{
    public interface IAircraftPaginatedQuery
    {
        string? Search { get; }

        AircraftStatus? Status { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
