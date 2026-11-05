using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.PreviewImport
{
    public interface IPreviewImportService
    {
        Task<BackofficeImportBatchDto> PreviewAsync(IPreviewImportCommand command, CancellationToken cancellationToken = default);
    }
}
