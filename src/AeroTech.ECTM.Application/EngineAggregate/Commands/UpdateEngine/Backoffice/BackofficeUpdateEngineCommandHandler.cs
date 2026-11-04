using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine.Backoffice
{
    public sealed class BackofficeUpdateEngineCommandHandler : IRequestHandler<BackofficeUpdateEngineCommand, BackofficeEngineDetailDto>
    {
        private readonly IUpdateEngineService _service;

        public BackofficeUpdateEngineCommandHandler(IUpdateEngineService service) => _service = service;

        public Task<BackofficeEngineDetailDto> Handle(BackofficeUpdateEngineCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
