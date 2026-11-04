using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using AeroTech.ECTM.Application.MaintenanceEventAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Arguments;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent
{
    public sealed class UpdateMaintenanceEventService : IUpdateMaintenanceEventService
    {
        private readonly IMaintenanceEventRepository _maintenanceEvents;
        private readonly MaintenanceEventDtoLoader _maintenanceEventDtos;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMaintenanceEventService(
            IMaintenanceEventRepository maintenanceEvents,
            MaintenanceEventDtoLoader maintenanceEventDtos,
            IUnitOfWork unitOfWork)
        {
            _maintenanceEvents = maintenanceEvents;
            _maintenanceEventDtos = maintenanceEventDtos;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeMaintenanceEventDto> UpdateAsync(IUpdateMaintenanceEventCommand command, CancellationToken cancellationToken = default)
        {
            var maintenanceEvent = await _maintenanceEvents.GetAsync(command.MaintenanceEventId, cancellationToken)
                                   ?? throw ExceptionFactory.MaintenanceEventNotFound(command.MaintenanceEventId);

            var args = new MaintenanceEventArgs(
                command.EventType,
                command.EventUtc,
                command.DateReliability,
                command.HoursAtEvent,
                command.CyclesAtEvent,
                command.WorkOrderReference,
                command.Notes);

            maintenanceEvent.Update(args);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _maintenanceEventDtos.LoadAsync(maintenanceEvent, cancellationToken);
        }
    }
}
