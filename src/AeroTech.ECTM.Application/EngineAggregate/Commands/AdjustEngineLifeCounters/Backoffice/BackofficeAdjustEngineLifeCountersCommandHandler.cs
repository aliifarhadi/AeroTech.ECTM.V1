using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.AdjustEngineLifeCounters.Backoffice
{
    public sealed class BackofficeAdjustEngineLifeCountersCommandHandler : IRequestHandler<BackofficeAdjustEngineLifeCountersCommand, BackofficeEngineDetailDto>
    {
        private readonly IAdjustEngineLifeCountersService _service;

        public BackofficeAdjustEngineLifeCountersCommandHandler(IAdjustEngineLifeCountersService service) => _service = service;

        public Task<BackofficeEngineDetailDto> Handle(BackofficeAdjustEngineLifeCountersCommand command, CancellationToken cancellationToken)
            => _service.AdjustAsync(command, cancellationToken);
    }
}
