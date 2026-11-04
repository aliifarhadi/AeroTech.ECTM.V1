using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Queries;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftPaginated
{
    public sealed class GetAircraftPaginatedService : IGetAircraftPaginatedService
    {
        private readonly IAircraftRepository _aircraft;
        private readonly IEngineInstallationRepository _installations;
        private readonly EngineInstallationDtoLoader _installationDtos;

        public GetAircraftPaginatedService(IAircraftRepository aircraft, IEngineInstallationRepository installations, EngineInstallationDtoLoader installationDtos)
        {
            _aircraft = aircraft;
            _installations = installations;
            _installationDtos = installationDtos;
        }

        public async Task<PaginatedList<BackofficeAircraftDto>> ExecuteAsync(IAircraftPaginatedQuery query, CancellationToken cancellationToken = default)
        {
            var page = await _aircraft.ListAsync(query.Search, query.Status, query.PageNumber, query.PageSize, cancellationToken);

            var aircraftIds = page.Results.Select(aircraft => aircraft.Id).ToList();
            var openInstallations = await _installations.ListOpenByAircraftIdsAsync(aircraftIds, cancellationToken);
            var installationDtos = await _installationDtos.LoadAsync(openInstallations, cancellationToken);

            return page.Map(aircraft => BackofficeAircraftDto.From(aircraft, installationDtos));
        }
    }
}
