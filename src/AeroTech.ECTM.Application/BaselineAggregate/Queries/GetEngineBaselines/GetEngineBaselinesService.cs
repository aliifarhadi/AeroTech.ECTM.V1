using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines
{
    public sealed class GetEngineBaselinesService : IGetEngineBaselinesService
    {
        private readonly IBaselineRepository _baselines;
        private readonly IProcessingJobRepository _jobs;
        private readonly BaselineDtoLoader _baselineDtos;

        public GetEngineBaselinesService(IBaselineRepository baselines, IProcessingJobRepository jobs, BaselineDtoLoader baselineDtos)
        {
            _baselines = baselines;
            _jobs = jobs;
            _baselineDtos = baselineDtos;
        }

        public async Task<BackofficeEngineBaselinesDto> ExecuteAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var serialNumber = await _baselineDtos.SerialNumberOfAsync(engineId, cancellationToken)
                               ?? throw ExceptionFactory.EngineNotFound(engineId);

            var baselines = await _baselines.ListForEngineAsync(engineId, cancellationToken);
            var baselineDtos = await _baselineDtos.LoadForEngineAsync(engineId, baselines, cancellationToken);
            var latestEvaluation = await _jobs.FindLatestAsync(ProcessingJobKind.BASELINE_EVALUATION, engineId, cancellationToken);
            var latestEvaluationDto = latestEvaluation is null ? null : BackofficeProcessingJobDto.From(latestEvaluation);

            return new BackofficeEngineBaselinesDto(engineId, serialNumber, latestEvaluationDto, baselineDtos);
        }
    }
}
