using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent
{
    public interface IUpdateMaintenanceEventService
    {
        Task<BackofficeMaintenanceEventDto> UpdateAsync(IUpdateMaintenanceEventCommand command, CancellationToken cancellationToken = default);
    }
}
