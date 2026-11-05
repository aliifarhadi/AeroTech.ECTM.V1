using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport.Backoffice
{
    public sealed class BackofficeCommitImportCommandHandler : IRequestHandler<BackofficeCommitImportCommand, BackofficeImportBatchDto>
    {
        private readonly ICommitImportService _service;

        public BackofficeCommitImportCommandHandler(ICommitImportService service) => _service = service;

        public Task<BackofficeImportBatchDto> Handle(BackofficeCommitImportCommand command, CancellationToken cancellationToken)
            => _service.CommitAsync(command, cancellationToken);
    }
}
