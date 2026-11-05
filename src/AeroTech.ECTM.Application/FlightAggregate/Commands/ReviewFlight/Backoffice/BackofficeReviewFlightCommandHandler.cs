using AeroTech.ECTM.Application.FlightAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ReviewFlight.Backoffice
{
    public sealed class BackofficeReviewFlightCommandHandler : IRequestHandler<BackofficeReviewFlightCommand, BackofficeFlightDetailDto>
    {
        private readonly IReviewFlightService _service;

        public BackofficeReviewFlightCommandHandler(IReviewFlightService service) => _service = service;

        public Task<BackofficeFlightDetailDto> Handle(BackofficeReviewFlightCommand command, CancellationToken cancellationToken)
            => _service.ReviewAsync(command, cancellationToken);
    }
}
