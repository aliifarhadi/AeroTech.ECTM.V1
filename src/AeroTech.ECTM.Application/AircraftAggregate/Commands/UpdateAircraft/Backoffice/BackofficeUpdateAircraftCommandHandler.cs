using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft.Backoffice
{
    public sealed class BackofficeUpdateAircraftCommandHandler : IRequestHandler<BackofficeUpdateAircraftCommand, BackofficeAircraftDto>
    {
        private readonly IUpdateAircraftService _service;

        public BackofficeUpdateAircraftCommandHandler(IUpdateAircraftService service) => _service = service;

        public Task<BackofficeAircraftDto> Handle(BackofficeUpdateAircraftCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
