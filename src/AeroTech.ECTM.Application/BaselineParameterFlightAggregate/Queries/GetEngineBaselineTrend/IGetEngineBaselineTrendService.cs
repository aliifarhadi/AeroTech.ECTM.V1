using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend
{
    public interface IGetEngineBaselineTrendService
    {
        Task<PaginatedList<BackofficeBaselineEvaluationDto>> ExecuteAsync(long engineId, EvaluationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
