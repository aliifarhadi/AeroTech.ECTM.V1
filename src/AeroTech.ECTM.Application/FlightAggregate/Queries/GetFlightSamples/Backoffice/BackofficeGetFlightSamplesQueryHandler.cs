using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples.Backoffice
{
    public sealed class BackofficeGetFlightSamplesQueryHandler : IRequestHandler<BackofficeGetFlightSamplesQuery, BackofficeFlightSamplesDto>
    {
        private readonly IGetFlightSamplesService _service;

        public BackofficeGetFlightSamplesQueryHandler(IGetFlightSamplesService service) => _service = service;

        public Task<BackofficeFlightSamplesDto> Handle(BackofficeGetFlightSamplesQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
