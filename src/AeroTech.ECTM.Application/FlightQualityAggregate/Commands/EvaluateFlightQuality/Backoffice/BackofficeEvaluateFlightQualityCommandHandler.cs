using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Commands.EvaluateFlightQuality.Backoffice
{
    public sealed class BackofficeEvaluateFlightQualityCommandHandler : IRequestHandler<BackofficeEvaluateFlightQualityCommand, BackofficeFlightQualityDto>
    {
        private readonly IEvaluateFlightQualityService _service;

        public BackofficeEvaluateFlightQualityCommandHandler(IEvaluateFlightQualityService service) => _service = service;

        public Task<BackofficeFlightQualityDto> Handle(BackofficeEvaluateFlightQualityCommand command, CancellationToken cancellationToken)
            => _service.EvaluateAsync(command.FlightId, cancellationToken);
    }
}
