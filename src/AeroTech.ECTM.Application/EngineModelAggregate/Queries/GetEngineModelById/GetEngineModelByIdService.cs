using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById
{
    public sealed class GetEngineModelByIdService : IGetEngineModelByIdService
    {
        private readonly IEngineModelRepository _engineModels;

        public GetEngineModelByIdService(IEngineModelRepository engineModels) => _engineModels = engineModels;

        public async Task<BackofficeEngineModelDto> ExecuteAsync(long engineModelId, CancellationToken cancellationToken = default)
        {
            var engineModel = await _engineModels.GetAsync(engineModelId, cancellationToken)
                              ?? throw ExceptionFactory.EngineModelNotFound(engineModelId);

            return BackofficeEngineModelDto.From(engineModel);
        }
    }
}
