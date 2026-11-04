using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent
{
    public interface ICreateMaintenanceEventService
    {
        Task<BackofficeMaintenanceEventDto> CreateAsync(ICreateMaintenanceEventCommand command, CancellationToken cancellationToken = default);
    }
}
