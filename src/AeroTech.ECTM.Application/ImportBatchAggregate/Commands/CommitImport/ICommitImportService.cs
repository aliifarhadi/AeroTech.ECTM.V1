using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport
{
    public interface ICommitImportService
    {
        Task<BackofficeImportBatchDto> CommitAsync(ICommitImportCommand command, CancellationToken cancellationToken = default);
    }
}
