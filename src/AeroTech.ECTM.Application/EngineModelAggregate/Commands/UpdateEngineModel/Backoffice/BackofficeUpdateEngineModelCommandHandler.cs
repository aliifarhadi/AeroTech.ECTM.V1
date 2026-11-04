using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel.Backoffice
{
    public sealed class BackofficeUpdateEngineModelCommandHandler : IRequestHandler<BackofficeUpdateEngineModelCommand, BackofficeEngineModelDto>
    {
        private readonly IUpdateEngineModelService _service;

        public BackofficeUpdateEngineModelCommandHandler(IUpdateEngineModelService service) => _service = service;

        public Task<BackofficeEngineModelDto> Handle(BackofficeUpdateEngineModelCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
