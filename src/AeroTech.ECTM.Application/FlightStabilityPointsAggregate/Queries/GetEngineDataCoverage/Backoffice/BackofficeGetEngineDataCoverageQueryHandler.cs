using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage.Backoffice
{
    public sealed class BackofficeGetEngineDataCoverageQueryHandler : IRequestHandler<BackofficeGetEngineDataCoverageQuery, BackofficeEngineDataCoverageDto>
    {
        private readonly IGetEngineDataCoverageService _service;

        public BackofficeGetEngineDataCoverageQueryHandler(IGetEngineDataCoverageService service) => _service = service;

        public Task<BackofficeEngineDataCoverageDto> Handle(BackofficeGetEngineDataCoverageQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineId, query.From, query.To, cancellationToken);
    }
}
