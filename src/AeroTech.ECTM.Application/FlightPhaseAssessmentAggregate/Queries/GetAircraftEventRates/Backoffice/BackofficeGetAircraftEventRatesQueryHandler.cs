using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates.Backoffice
{
    public sealed class BackofficeGetAircraftEventRatesQueryHandler : IRequestHandler<BackofficeGetAircraftEventRatesQuery, BackofficeAircraftEventRatesDto>
    {
        private readonly IGetAircraftEventRatesService _service;

        public BackofficeGetAircraftEventRatesQueryHandler(IGetAircraftEventRatesService service) => _service = service;

        public Task<BackofficeAircraftEventRatesDto> Handle(BackofficeGetAircraftEventRatesQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.AircraftId, query.From, query.To, cancellationToken);
    }
}
