using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity.Backoffice
{
    public sealed class BackofficeConfirmFlightIdentityCommandHandler : IRequestHandler<BackofficeConfirmFlightIdentityCommand, BackofficeFlightDetailDto>
    {
        private readonly IConfirmFlightIdentityService _service;

        public BackofficeConfirmFlightIdentityCommandHandler(IConfirmFlightIdentityService service) => _service = service;

        public Task<BackofficeFlightDetailDto> Handle(BackofficeConfirmFlightIdentityCommand command, CancellationToken cancellationToken)
            => _service.ConfirmAsync(command, cancellationToken);
    }
}
