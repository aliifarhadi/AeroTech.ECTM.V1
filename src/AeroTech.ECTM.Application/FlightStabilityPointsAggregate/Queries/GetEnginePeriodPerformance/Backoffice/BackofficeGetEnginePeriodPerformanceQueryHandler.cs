using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance.Backoffice
{
    public sealed class BackofficeGetEnginePeriodPerformanceQueryHandler : IRequestHandler<BackofficeGetEnginePeriodPerformanceQuery, BackofficePeriodPerformanceDto>
    {
        private readonly IGetEnginePeriodPerformanceService _service;

        public BackofficeGetEnginePeriodPerformanceQueryHandler(IGetEnginePeriodPerformanceService service) => _service = service;

        public Task<BackofficePeriodPerformanceDto> Handle(BackofficeGetEnginePeriodPerformanceQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineId, query.From, query.To, query.Kind, query.Selector, cancellationToken);
    }
}
