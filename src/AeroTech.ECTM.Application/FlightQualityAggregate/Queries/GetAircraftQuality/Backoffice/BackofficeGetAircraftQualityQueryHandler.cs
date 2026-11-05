using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality.Backoffice
{
    public sealed class BackofficeGetAircraftQualityQueryHandler : IRequestHandler<BackofficeGetAircraftQualityQuery, BackofficeAircraftQualityDto>
    {
        private readonly IGetAircraftQualityService _service;

        public BackofficeGetAircraftQualityQueryHandler(IGetAircraftQualityService service) => _service = service;

        public Task<BackofficeAircraftQualityDto> Handle(BackofficeGetAircraftQualityQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.AircraftId, query.From, query.To, cancellationToken);
    }
}
