using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated.Backoffice
{
    public sealed class BackofficeGetAircraftPaginatedQueryHandler : IRequestHandler<BackofficeGetAircraftPaginatedQuery, PaginatedList<BackofficeAircraftDto>>
    {
        private readonly IGetAircraftPaginatedService _service;

        public BackofficeGetAircraftPaginatedQueryHandler(IGetAircraftPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeAircraftDto>> Handle(BackofficeGetAircraftPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
