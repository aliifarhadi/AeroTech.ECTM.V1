using AeroTech.ECTM.Application.BaselineAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PrefillBaselineFromFile.Backoffice
{
    public sealed class BackofficePrefillBaselineFromFileCommandHandler : IRequestHandler<BackofficePrefillBaselineFromFileCommand, BackofficeBaselinePrefillDto>
    {
        private readonly IPrefillBaselineFromFileService _service;

        public BackofficePrefillBaselineFromFileCommandHandler(IPrefillBaselineFromFileService service) => _service = service;

        public Task<BackofficeBaselinePrefillDto> Handle(BackofficePrefillBaselineFromFileCommand command, CancellationToken cancellationToken)
            => _service.PrefillAsync(command, cancellationToken);
    }
}
