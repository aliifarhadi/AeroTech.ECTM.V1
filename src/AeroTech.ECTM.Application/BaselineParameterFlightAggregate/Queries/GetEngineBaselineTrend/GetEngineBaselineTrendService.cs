using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetEngineBaselineTrend
{
    public sealed class GetEngineBaselineTrendService : IGetEngineBaselineTrendService
    {
        private readonly IEngineRepository _engines;
        private readonly IBaselineParameterFlightRepository _evaluations;
        private readonly BaselineEvaluationDtoLoader _evaluationDtos;

        public GetEngineBaselineTrendService(IEngineRepository engines, IBaselineParameterFlightRepository evaluations, BaselineEvaluationDtoLoader evaluationDtos)
        {
            _engines = engines;
            _evaluations = evaluations;
            _evaluationDtos = evaluationDtos;
        }

        public async Task<PaginatedList<BackofficeBaselineEvaluationDto>> ExecuteAsync(long engineId, EvaluationFilterArgs filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!await _engines.ExistsAsync(engineId, cancellationToken))
                throw ExceptionFactory.EngineNotFound(engineId);

            var page = await _evaluations.ListTrendAsync(engineId, filter, pageNumber, pageSize, cancellationToken);
            var evaluationDtos = await _evaluationDtos.LoadAsync(page.Results.ToList(), cancellationToken);
            var dtoById = evaluationDtos.ToDictionary(evaluation => evaluation.Id!.Value);

            return page.Map(evaluation => dtoById[evaluation.Id]);
        }
    }
}
