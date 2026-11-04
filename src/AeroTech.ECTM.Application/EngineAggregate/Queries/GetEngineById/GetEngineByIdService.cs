using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById
{
    public sealed class GetEngineByIdService : IGetEngineByIdService
    {
        private readonly IEngineRepository _engines;
        private readonly IEngineModelRepository _engineModels;
        private readonly IEngineInstallationRepository _installations;
        private readonly EngineInstallationDtoLoader _installationDtos;

        public GetEngineByIdService(
            IEngineRepository engines,
            IEngineModelRepository engineModels,
            IEngineInstallationRepository installations,
            EngineInstallationDtoLoader installationDtos)
        {
            _engines = engines;
            _engineModels = engineModels;
            _installations = installations;
            _installationDtos = installationDtos;
        }

        public async Task<BackofficeEngineDetailDto> ExecuteAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(engineId, cancellationToken)
                         ?? throw ExceptionFactory.EngineNotFound(engineId);

            var engineModel = await _engineModels.GetAsync(engine.EngineModelId, cancellationToken)
                              ?? throw ExceptionFactory.EngineModelNotFound(engine.EngineModelId);

            var openInstallations = await _installations.ListOpenByEngineIdsAsync([engineId], cancellationToken);
            var installationDtos = await _installationDtos.LoadAsync(openInstallations, cancellationToken);

            return BackofficeEngineDetailDto.From(engine, engineModel.Code, installationDtos.SingleOrDefault());
        }
    }
}
