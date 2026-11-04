using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelsPaginated.Backoffice
{
    public sealed class BackofficeGetEngineModelsPaginatedQueryHandler : IRequestHandler<BackofficeGetEngineModelsPaginatedQuery, PaginatedList<BackofficeEngineModelDto>>
    {
        private readonly IGetEngineModelsPaginatedService _service;

        public BackofficeGetEngineModelsPaginatedQueryHandler(IGetEngineModelsPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeEngineModelDto>> Handle(BackofficeGetEngineModelsPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
