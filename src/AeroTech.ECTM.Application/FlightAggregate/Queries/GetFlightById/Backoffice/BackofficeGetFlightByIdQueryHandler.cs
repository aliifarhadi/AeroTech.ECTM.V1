using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightById.Backoffice
{
    public sealed class BackofficeGetFlightByIdQueryHandler : IRequestHandler<BackofficeGetFlightByIdQuery, BackofficeFlightDetailDto>
    {
        private readonly IGetFlightByIdService _service;

        public BackofficeGetFlightByIdQueryHandler(IGetFlightByIdService service) => _service = service;

        public Task<BackofficeFlightDetailDto> Handle(BackofficeGetFlightByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
