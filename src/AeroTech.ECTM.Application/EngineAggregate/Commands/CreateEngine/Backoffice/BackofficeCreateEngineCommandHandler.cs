using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine.Backoffice
{
    public sealed class BackofficeCreateEngineCommandHandler : IRequestHandler<BackofficeCreateEngineCommand, BackofficeEngineDetailDto>
    {
        private readonly ICreateEngineService _service;

        public BackofficeCreateEngineCommandHandler(ICreateEngineService service) => _service = service;

        public Task<BackofficeEngineDetailDto> Handle(BackofficeCreateEngineCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
