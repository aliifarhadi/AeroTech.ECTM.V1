using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport.Backoffice
{
    public sealed class BackofficePreviewImportCommandHandler : IRequestHandler<BackofficePreviewImportCommand, BackofficeImportBatchDto>
    {
        private readonly IPreviewImportService _service;

        public BackofficePreviewImportCommandHandler(IPreviewImportService service) => _service = service;

        public Task<BackofficeImportBatchDto> Handle(BackofficePreviewImportCommand command, CancellationToken cancellationToken)
            => _service.PreviewAsync(command, cancellationToken);
    }
}
