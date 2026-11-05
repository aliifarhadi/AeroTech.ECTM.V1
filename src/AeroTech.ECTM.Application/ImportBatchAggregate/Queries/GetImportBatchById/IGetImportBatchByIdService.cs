using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById
{
    public interface IGetImportBatchByIdService
    {
        Task<BackofficeImportBatchDto> ExecuteAsync(long batchId, CancellationToken cancellationToken = default);
    }
}
