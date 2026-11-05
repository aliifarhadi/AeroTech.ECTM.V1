using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance.Backoffice
{
    public sealed class BackofficeGetModelPeriodPerformanceQueryHandler : IRequestHandler<BackofficeGetModelPeriodPerformanceQuery, BackofficeModelPeriodPerformanceDto>
    {
        private readonly IGetModelPeriodPerformanceService _service;

        public BackofficeGetModelPeriodPerformanceQueryHandler(IGetModelPeriodPerformanceService service) => _service = service;

        public Task<BackofficeModelPeriodPerformanceDto> Handle(BackofficeGetModelPeriodPerformanceQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineModelId, query.From, query.To, query.Kind, query.Selector, cancellationToken);
    }
}
