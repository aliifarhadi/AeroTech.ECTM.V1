using AeroTech.ECTM.Application.MaintenanceEventAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Commands.CreateMaintenanceEvent.Backoffice
{
    public sealed class BackofficeCreateMaintenanceEventCommandHandler : IRequestHandler<BackofficeCreateMaintenanceEventCommand, BackofficeMaintenanceEventDto>
    {
        private readonly ICreateMaintenanceEventService _service;

        public BackofficeCreateMaintenanceEventCommandHandler(ICreateMaintenanceEventService service) => _service = service;

        public Task<BackofficeMaintenanceEventDto> Handle(BackofficeCreateMaintenanceEventCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
