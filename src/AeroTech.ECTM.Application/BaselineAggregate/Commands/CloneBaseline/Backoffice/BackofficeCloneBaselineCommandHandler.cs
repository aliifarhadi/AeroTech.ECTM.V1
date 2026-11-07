using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.CloneBaseline.Backoffice
{
    public sealed class BackofficeCloneBaselineCommandHandler : IRequestHandler<BackofficeCloneBaselineCommand, BackofficeBaselineDto>
    {
        private readonly ICloneBaselineService _service;

        public BackofficeCloneBaselineCommandHandler(ICloneBaselineService service) => _service = service;

        public Task<BackofficeBaselineDto> Handle(BackofficeCloneBaselineCommand command, CancellationToken cancellationToken)
            => _service.CloneAsync(command, cancellationToken);
    }
}
