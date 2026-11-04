using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated.Backoffice
{
    public sealed class BackofficeGetAircraftPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeAircraftDto>>, IAircraftPaginatedQuery
    {
        public string? Search { get; set; }

        public AircraftStatus? Status { get; set; }
    }
}
