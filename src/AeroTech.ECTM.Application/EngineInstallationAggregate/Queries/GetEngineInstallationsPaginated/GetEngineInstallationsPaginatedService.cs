using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Arguments;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated
{
    public sealed class GetEngineInstallationsPaginatedService : IGetEngineInstallationsPaginatedService
    {
        private readonly IEngineInstallationRepository _installations;
        private readonly EngineInstallationDtoLoader _installationDtos;

        public GetEngineInstallationsPaginatedService(IEngineInstallationRepository installations, EngineInstallationDtoLoader installationDtos)
        {
            _installations = installations;
            _installationDtos = installationDtos;
        }

        public async Task<PaginatedList<BackofficeEngineInstallationDto>> ExecuteAsync(IEngineInstallationsPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var filter = new InstallationFilterArgs(query.EngineId, query.AircraftId, query.Position, query.OpenOnly);
            var page = await _installations.ListAsync(filter, query.PageNumber, query.PageSize, cancellationToken);

            var installationDtos = await _installationDtos.LoadAsync(page.Results.ToList(), cancellationToken);
            var dtoById = installationDtos.ToDictionary(installation => installation.Id);

            return page.Map(installation => dtoById[installation.Id]);
        }
    }
}
