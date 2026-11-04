using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.UpdateMaintenanceEvent.Backoffice
{
    public sealed class BackofficeUpdateMaintenanceEventCommandHandler : IRequestHandler<BackofficeUpdateMaintenanceEventCommand, BackofficeMaintenanceEventDto>
    {
        private readonly IUpdateMaintenanceEventService _service;

        public BackofficeUpdateMaintenanceEventCommandHandler(IUpdateMaintenanceEventService service) => _service = service;

        public Task<BackofficeMaintenanceEventDto> Handle(BackofficeUpdateMaintenanceEventCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
