using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineAggregate.Contracts;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById
{
    public sealed class GetBaselineByIdService : IGetBaselineByIdService
    {
        private readonly IBaselineRepository _baselines;
        private readonly BaselineDtoLoader _baselineDtos;

        public GetBaselineByIdService(IBaselineRepository baselines, BaselineDtoLoader baselineDtos)
        {
            _baselines = baselines;
            _baselineDtos = baselineDtos;
        }

        public async Task<BackofficeBaselineDto> ExecuteAsync(long baselineId, CancellationToken cancellationToken = default)
        {
            var baseline = await _baselines.FindAsync(baselineId, cancellationToken)
                           ?? throw ExceptionFactory.BaselineNotFound(baselineId);

            return await _baselineDtos.LoadAsync(baseline, cancellationToken);
        }
    }
}
