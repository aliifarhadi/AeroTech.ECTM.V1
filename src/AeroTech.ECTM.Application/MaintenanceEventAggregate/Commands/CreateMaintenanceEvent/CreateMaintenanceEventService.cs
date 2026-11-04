using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Arguments;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent
{
    public sealed class CreateMaintenanceEventService : ICreateMaintenanceEventService
    {
        private readonly IMaintenanceEventRepository _maintenanceEvents;
        private readonly IEngineRepository _engines;
        private readonly IAircraftRepository _aircraft;
        private readonly MaintenanceEventDtoLoader _maintenanceEventDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;

        public CreateMaintenanceEventService(
            IMaintenanceEventRepository maintenanceEvents,
            IEngineRepository engines,
            IAircraftRepository aircraft,
            MaintenanceEventDtoLoader maintenanceEventDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator)
        {
            _maintenanceEvents = maintenanceEvents;
            _engines = engines;
            _aircraft = aircraft;
            _maintenanceEventDtos = maintenanceEventDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
        }

        public async Task<BackofficeMaintenanceEventDto> CreateAsync(ICreateMaintenanceEventCommand command, CancellationToken cancellationToken = default)
        {
            var args = new MaintenanceEventArgs(
                command.EventType,
                command.EventUtc,
                command.DateReliability,
                command.HoursAtEvent,
                command.CyclesAtEvent,
                command.WorkOrderReference,
                command.Notes);

            var maintenanceEvent = await NewMaintenanceEventAsync(command, args, cancellationToken);

            await _maintenanceEvents.AddAsync(maintenanceEvent, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _maintenanceEventDtos.LoadAsync(maintenanceEvent, cancellationToken);
        }

        private async Task<MaintenanceEvent> NewMaintenanceEventAsync(ICreateMaintenanceEventCommand command, MaintenanceEventArgs args, CancellationToken cancellationToken)
        {
            var namesEngine = command.EngineId is not null;
            var namesPosition = command.AircraftId is not null || command.Position is not null;

            if (namesEngine && namesPosition)
                throw ExceptionFactory.MaintenanceEventIdentityIsInvalid();

            if (command.EngineId is { } engineId)
            {
                if (!await _engines.ExistsAsync(engineId, cancellationToken))
                    throw ExceptionFactory.EngineNotFound(engineId);

                return MaintenanceEvent.ForEngine(_idGenerator.NewId(), engineId, args);
            }

            if (command.AircraftId is not { } aircraftId)
                throw ExceptionFactory.MaintenanceEventIdentityIsInvalid();

            if (command.Position is not { } position)
                throw ExceptionFactory.MaintenanceEventIdentityIsInvalid();

            if (!await _aircraft.ExistsAsync(aircraftId, cancellationToken))
                throw ExceptionFactory.AircraftNotFound(aircraftId);

            return MaintenanceEvent.ForPosition(_idGenerator.NewId(), aircraftId, position, args);
        }
    }
}
