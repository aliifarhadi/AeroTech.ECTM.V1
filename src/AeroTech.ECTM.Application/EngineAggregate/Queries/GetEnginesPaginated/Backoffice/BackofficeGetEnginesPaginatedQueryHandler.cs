using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated.Backoffice
{
    public sealed class BackofficeGetEnginesPaginatedQueryHandler : IRequestHandler<BackofficeGetEnginesPaginatedQuery, PaginatedList<BackofficeEngineDto>>
    {
        private readonly IGetEnginesPaginatedService _service;

        public BackofficeGetEnginesPaginatedQueryHandler(IGetEnginesPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeEngineDto>> Handle(BackofficeGetEnginesPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
