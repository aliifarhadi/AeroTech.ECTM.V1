using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetFlightControlContext.Backoffice
{
    public sealed class BackofficeGetFlightControlContextQueryHandler : IRequestHandler<BackofficeGetFlightControlContextQuery, BackofficeControlContextDto>
    {
        private readonly IGetFlightControlContextService _service;

        public BackofficeGetFlightControlContextQueryHandler(IGetFlightControlContextService service) => _service = service;

        public Task<BackofficeControlContextDto> Handle(BackofficeGetFlightControlContextQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
