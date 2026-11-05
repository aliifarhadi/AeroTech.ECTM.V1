using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated.Backoffice
{
    public sealed class BackofficeGetFlightsPaginatedQueryHandler : IRequestHandler<BackofficeGetFlightsPaginatedQuery, PaginatedList<BackofficeFlightSummaryDto>>
    {
        private readonly IGetFlightsPaginatedService _service;

        public BackofficeGetFlightsPaginatedQueryHandler(IGetFlightsPaginatedService service) => _service = service;

        public Task<PaginatedList<BackofficeFlightSummaryDto>> Handle(BackofficeGetFlightsPaginatedQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
