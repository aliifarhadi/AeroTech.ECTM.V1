using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline.Backoffice
{
    public sealed class BackofficeUpdateBaselineCommandHandler : IRequestHandler<BackofficeUpdateBaselineCommand, BackofficeBaselineDto>
    {
        private readonly IUpdateBaselineService _service;

        public BackofficeUpdateBaselineCommandHandler(IUpdateBaselineService service) => _service = service;

        public Task<BackofficeBaselineDto> Handle(BackofficeUpdateBaselineCommand command, CancellationToken cancellationToken)
            => _service.UpdateAsync(command, cancellationToken);
    }
}
