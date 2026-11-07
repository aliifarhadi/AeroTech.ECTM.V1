using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.RetireBaseline.Backoffice
{
    public sealed class BackofficeRetireBaselineCommandHandler : IRequestHandler<BackofficeRetireBaselineCommand, BackofficeBaselineDto>
    {
        private readonly IRetireBaselineService _service;

        public BackofficeRetireBaselineCommandHandler(IRetireBaselineService service) => _service = service;

        public Task<BackofficeBaselineDto> Handle(BackofficeRetireBaselineCommand command, CancellationToken cancellationToken)
            => _service.RetireAsync(command, cancellationToken);
    }
}
