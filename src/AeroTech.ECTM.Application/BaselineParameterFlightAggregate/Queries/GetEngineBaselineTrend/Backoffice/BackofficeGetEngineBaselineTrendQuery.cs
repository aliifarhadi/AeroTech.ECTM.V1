using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Queries;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend.Backoffice
{
    public sealed record BackofficeGetEngineBaselineTrendQuery(long EngineId, EvaluationFilterArgs Filter, int PageNumber, int PageSize) : IRequest<PaginatedList<BackofficeBaselineEvaluationDto>>;
}
