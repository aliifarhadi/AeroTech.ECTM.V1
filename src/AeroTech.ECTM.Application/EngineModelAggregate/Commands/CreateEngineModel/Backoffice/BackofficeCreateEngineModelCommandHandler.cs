using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel.Backoffice
{
    public sealed class BackofficeCreateEngineModelCommandHandler : IRequestHandler<BackofficeCreateEngineModelCommand, BackofficeEngineModelDto>
    {
        private readonly ICreateEngineModelService _service;

        public BackofficeCreateEngineModelCommandHandler(ICreateEngineModelService service) => _service = service;

        public Task<BackofficeEngineModelDto> Handle(BackofficeCreateEngineModelCommand command, CancellationToken cancellationToken)
            => _service.CreateAsync(command, cancellationToken);
    }
}
