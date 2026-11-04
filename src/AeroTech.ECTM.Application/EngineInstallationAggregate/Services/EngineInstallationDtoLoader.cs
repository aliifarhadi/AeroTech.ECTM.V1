using AeroTech.ECTM.Application.EngineInstallationAggregate.Dto;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Services
{
    public sealed class EngineInstallationDtoLoader
    {
        private readonly IEngineRepository _engines;
        private readonly IAircraftRepository _aircraft;

        public EngineInstallationDtoLoader(IEngineRepository engines, IAircraftRepository aircraft)
        {
            _engines = engines;
            _aircraft = aircraft;
        }

        public async Task<IReadOnlyList<BackofficeEngineInstallationDto>> LoadAsync(IReadOnlyCollection<EngineInstallation> installations, CancellationToken cancellationToken = default)
        {
            var engineIds = installations.Select(installation => installation.EngineId).Distinct().ToList();
            var aircraftIds = installations.Select(installation => installation.AircraftId).Distinct().ToList();

            var engines = await _engines.ListByIdsAsync(engineIds, cancellationToken);
            var aircraft = await _aircraft.ListByIdsAsync(aircraftIds, cancellationToken);

            var serialByEngine = engines.ToDictionary(engine => engine.Id, engine => engine.SerialNumber);
            var registrationByAircraft = aircraft.ToDictionary(item => item.Id, item => item.Registration);

            return installations
                .Select(installation => BackofficeEngineInstallationDto.From(
                    installation,
                    serialByEngine[installation.EngineId],
                    registrationByAircraft[installation.AircraftId]))
                .ToList();
        }

        public async Task<BackofficeEngineInstallationDto> LoadAsync(EngineInstallation installation, CancellationToken cancellationToken = default)
        {
            var dtos = await LoadAsync([installation], cancellationToken);

            return dtos.Single();
        }
    }
}
