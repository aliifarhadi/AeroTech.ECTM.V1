using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Services
{
    public sealed class MaintenanceEventDtoLoader
    {
        private readonly IEngineRepository _engines;
        private readonly IAircraftRepository _aircraft;

        public MaintenanceEventDtoLoader(IEngineRepository engines, IAircraftRepository aircraft)
        {
            _engines = engines;
            _aircraft = aircraft;
        }

        public async Task<IReadOnlyList<BackofficeMaintenanceEventDto>> LoadAsync(IReadOnlyCollection<MaintenanceEvent> maintenanceEvents, CancellationToken cancellationToken = default)
        {
            var engineIds = maintenanceEvents.Where(item => item.EngineId is not null).Select(item => item.EngineId!.Value).Distinct().ToList();
            var aircraftIds = maintenanceEvents.Where(item => item.AircraftId is not null).Select(item => item.AircraftId!.Value).Distinct().ToList();

            var engines = await _engines.ListByIdsAsync(engineIds, cancellationToken);
            var aircraft = await _aircraft.ListByIdsAsync(aircraftIds, cancellationToken);

            var serialByEngine = engines.ToDictionary(engine => engine.Id, engine => engine.SerialNumber);
            var registrationByAircraft = aircraft.ToDictionary(item => item.Id, item => item.Registration);

            var dtos = new List<BackofficeMaintenanceEventDto>();

            foreach (var maintenanceEvent in maintenanceEvents)
            {
                string? serialNumber = null;
                string? registration = null;

                if (maintenanceEvent.EngineId is { } engineId)
                    serialNumber = serialByEngine[engineId];

                if (maintenanceEvent.AircraftId is { } aircraftId)
                    registration = registrationByAircraft[aircraftId];

                dtos.Add(BackofficeMaintenanceEventDto.From(maintenanceEvent, serialNumber, registration));
            }

            return dtos;
        }

        public async Task<BackofficeMaintenanceEventDto> LoadAsync(MaintenanceEvent maintenanceEvent, CancellationToken cancellationToken = default)
        {
            var dtos = await LoadAsync([maintenanceEvent], cancellationToken);

            return dtos.Single();
        }
    }
}
