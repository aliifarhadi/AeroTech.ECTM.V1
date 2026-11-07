using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline.Backoffice
{
    public sealed class BackofficeSetPrimaryBaselineCommandHandler : IRequestHandler<BackofficeSetPrimaryBaselineCommand, BackofficeEngineBaselinesDto>
    {
        private readonly ISetPrimaryBaselineService _service;

        public BackofficeSetPrimaryBaselineCommandHandler(ISetPrimaryBaselineService service) => _service = service;

        public Task<BackofficeEngineBaselinesDto> Handle(BackofficeSetPrimaryBaselineCommand command, CancellationToken cancellationToken)
            => _service.SetAsync(command, cancellationToken);
    }
}
