using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetFlightQuality.Backoffice
{
    public sealed class BackofficeGetFlightQualityQueryHandler : IRequestHandler<BackofficeGetFlightQualityQuery, BackofficeFlightQualityDto>
    {
        private readonly IGetFlightQualityService _service;

        public BackofficeGetFlightQualityQueryHandler(IGetFlightQualityService service) => _service = service;

        public Task<BackofficeFlightQualityDto> Handle(BackofficeGetFlightQualityQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, cancellationToken);
    }
}
