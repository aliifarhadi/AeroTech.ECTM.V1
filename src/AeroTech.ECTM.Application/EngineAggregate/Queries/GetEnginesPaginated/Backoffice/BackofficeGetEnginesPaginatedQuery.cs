using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated.Backoffice
{
    public sealed class BackofficeGetEnginesPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeEngineDto>>, IEnginesPaginatedQuery
    {
        public string? Search { get; set; }

        public long? EngineModelId { get; set; }

        public EngineStatus? Status { get; set; }
    }
}
