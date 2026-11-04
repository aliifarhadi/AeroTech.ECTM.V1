using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft.Backoffice
{
    public sealed class BackofficeCreateAircraftCommandHandler : IRequestHandler<BackofficeCreateAircraftCommand, BackofficeAircraftDto>
    {
        private readonly ICreateAircraftService _service;

        public BackofficeCreateAircraftCommandHandler(ICreateAircraftService service) => _service = service;

        public Task<BackofficeAircraftDto> Handle(BackofficeCreateAircraftCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
