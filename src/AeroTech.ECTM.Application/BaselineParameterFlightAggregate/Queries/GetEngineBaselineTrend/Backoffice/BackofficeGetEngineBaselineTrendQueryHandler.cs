using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend.Backoffice
{
    public sealed class BackofficeGetEngineBaselineTrendQueryHandler : IRequestHandler<BackofficeGetEngineBaselineTrendQuery, PaginatedList<BackofficeBaselineEvaluationDto>>
    {
        private readonly IGetEngineBaselineTrendService _service;

        public BackofficeGetEngineBaselineTrendQueryHandler(IGetEngineBaselineTrendService service) => _service = service;

        public Task<PaginatedList<BackofficeBaselineEvaluationDto>> Handle(BackofficeGetEngineBaselineTrendQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineId, query.Filter, query.PageNumber, query.PageSize, cancellationToken);
    }
}
