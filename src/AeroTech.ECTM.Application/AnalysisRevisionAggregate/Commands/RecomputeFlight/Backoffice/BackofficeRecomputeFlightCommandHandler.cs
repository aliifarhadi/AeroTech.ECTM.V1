using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeFlight.Backoffice
{
    public sealed class BackofficeRecomputeFlightCommandHandler : IRequestHandler<BackofficeRecomputeFlightCommand, BackofficeRecomputeDto>
    {
        private readonly IRecomputeFlightService _service;

        public BackofficeRecomputeFlightCommandHandler(IRecomputeFlightService service) => _service = service;

        public Task<BackofficeRecomputeDto> Handle(BackofficeRecomputeFlightCommand command, CancellationToken cancellationToken)
            => _service.RecomputeAsync(command, cancellationToken);
    }
}
