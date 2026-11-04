using AeroTech.ECTM.Application.EngineAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineModelAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEnginesPaginated
{
    public sealed class GetEnginesPaginatedService : IGetEnginesPaginatedService
    {
        private readonly IEngineRepository _engines;
        private readonly IEngineModelRepository _engineModels;
        private readonly IEngineInstallationRepository _installations;
        private readonly EngineInstallationDtoLoader _installationDtos;

        public GetEnginesPaginatedService(
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

        public async Task<PaginatedList<BackofficeEngineDto>> ExecuteAsync(IEnginesPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var page = await _engines.ListAsync(query.Search, query.EngineModelId, query.Status, query.PageNumber, query.PageSize, cancellationToken);

            var engineIds = page.Results.Select(engine => engine.Id).ToList();
            var engineModelIds = page.Results.Select(engine => engine.EngineModelId).Distinct().ToList();

            var engineModels = await _engineModels.ListByIdsAsync(engineModelIds, cancellationToken);
            var codeByModel = engineModels.ToDictionary(engineModel => engineModel.Id, engineModel => engineModel.Code);

            var openInstallations = await _installations.ListOpenByEngineIdsAsync(engineIds, cancellationToken);
            var installationDtos = await _installationDtos.LoadAsync(openInstallations, cancellationToken);
            var installationByEngine = installationDtos.ToDictionary(installation => installation.EngineId);

            return page.Map(engine => BackofficeEngineDto.From(
                engine,
                codeByModel[engine.EngineModelId],
                installationByEngine.GetValueOrDefault(engine.Id)));
        }
    }
}
