using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated.Backoffice
{
    public sealed class BackofficeGetEngineInstallationsPaginatedQueryHandler : IRequestHandler<BackofficeGetEngineInstallationsPaginatedQuery, PaginatedList<BackofficeEngineInstallationDto>>
    {
        private readonly IGetEngineInstallationsPaginatedService _service;

        public BackofficeGetEngineInstallationsPaginatedQueryHandler(IGetEngineInstallationsPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeEngineInstallationDto>> Handle(BackofficeGetEngineInstallationsPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
