using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated
{
    public sealed class GetEngineModelsPaginatedService : IGetEngineModelsPaginatedService
    {
        private readonly IEngineModelRepository _engineModels;

        public GetEngineModelsPaginatedService(IEngineModelRepository engineModels) => _engineModels = engineModels;

        public async Task<PaginatedList<BackofficeEngineModelDto>> ExecuteAsync(IEngineModelsPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var page = await _engineModels.ListAsync(query.Search, query.PageNumber, query.PageSize, cancellationToken);

            return page.Map(BackofficeEngineModelDto.From);
        }
    }
}
