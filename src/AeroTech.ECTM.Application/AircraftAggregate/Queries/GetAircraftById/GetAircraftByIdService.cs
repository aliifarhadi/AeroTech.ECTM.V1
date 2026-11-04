using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.ECTM.Application.EngineInstallationAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById
{
    public sealed class GetAircraftByIdService : IGetAircraftByIdService
    {
        private readonly IAircraftRepository _aircraft;
        private readonly IEngineInstallationRepository _installations;
        private readonly EngineInstallationDtoLoader _installationDtos;

        public GetAircraftByIdService(IAircraftRepository aircraft, IEngineInstallationRepository installations, EngineInstallationDtoLoader installationDtos)
        {
            _aircraft = aircraft;
            _installations = installations;
            _installationDtos = installationDtos;
        }

        public async Task<BackofficeAircraftDto> ExecuteAsync(long aircraftId, CancellationToken cancellationToken = default)
        {
            var aircraft = await _aircraft.GetAsync(aircraftId, cancellationToken)
                           ?? throw ExceptionFactory.AircraftNotFound(aircraftId);

            var openInstallations = await _installations.ListOpenByAircraftIdsAsync([aircraftId], cancellationToken);
            var installationDtos = await _installationDtos.LoadAsync(openInstallations, cancellationToken);

            return BackofficeAircraftDto.From(aircraft, installationDtos);
        }
    }
}
