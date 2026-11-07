using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons.Backoffice
{
    public sealed class BackofficeGetFlightBaselineComparisonsQueryHandler : IRequestHandler<BackofficeGetFlightBaselineComparisonsQuery, BackofficeFlightBaselineComparisonsDto>
    {
        private readonly IGetFlightBaselineComparisonsService _service;

        public BackofficeGetFlightBaselineComparisonsQueryHandler(IGetFlightBaselineComparisonsService service) => _service = service;

        public Task<BackofficeFlightBaselineComparisonsDto> Handle(BackofficeGetFlightBaselineComparisonsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.FlightId, query.Filter, cancellationToken);
    }
}
