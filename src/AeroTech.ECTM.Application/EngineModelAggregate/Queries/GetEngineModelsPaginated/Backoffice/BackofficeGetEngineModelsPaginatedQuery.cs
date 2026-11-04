using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated.Backoffice
{
    public sealed class BackofficeGetEngineModelsPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeEngineModelDto>>, IEngineModelsPaginatedQuery
    {
        public string? Search { get; set; }
    }
}
