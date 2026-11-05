using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightStabilityPoints.Backoffice
{
    public sealed class BackofficeGetFlightStabilityPointsQueryHandler : IRequestHandler<BackofficeGetFlightStabilityPointsQuery, BackofficeFlightStabilityPointsDto>
    {
        private readonly IGetFlightStabilityPointsService _service;

        public BackofficeGetFlightStabilityPointsQueryHandler(IGetFlightStabilityPointsService service) => _service = service;

        public Task<BackofficeFlightStabilityPointsDto> Handle(BackofficeGetFlightStabilityPointsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
