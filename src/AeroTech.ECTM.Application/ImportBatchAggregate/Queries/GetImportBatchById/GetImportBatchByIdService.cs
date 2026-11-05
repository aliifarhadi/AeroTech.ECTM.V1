using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById
{
    public sealed class GetImportBatchByIdService : IGetImportBatchByIdService
    {
        private readonly IImportBatchRepository _batches;
        private readonly ImportBatchDtoLoader _batchDtos;

        public GetImportBatchByIdService(IImportBatchRepository batches, ImportBatchDtoLoader batchDtos)
        {
            _batches = batches;
            _batchDtos = batchDtos;
        }

        public async Task<BackofficeImportBatchDto> ExecuteAsync(long batchId, CancellationToken cancellationToken = default)
        {
            var batch = await _batches.FindAsync(batchId, cancellationToken)
                        ?? throw ExceptionFactory.ImportBatchNotFound(batchId);

            return await _batchDtos.LoadAsync(batch, new Dictionary<long, ShownRows>(), cancellationToken);
        }
    }
}
