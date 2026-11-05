using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries.Backoffice
{
    public sealed class BackofficeGetFlightSeriesQueryHandler : IRequestHandler<BackofficeGetFlightSeriesQuery, BackofficeFlightSeriesDto>
    {
        private readonly IGetFlightSeriesService _service;

        public BackofficeGetFlightSeriesQueryHandler(IGetFlightSeriesService service) => _service = service;

        public Task<BackofficeFlightSeriesDto> Handle(BackofficeGetFlightSeriesQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
