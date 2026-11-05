using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Contracts;
using AeroTech.ECTM.Domain.ProcessingJobAggregate;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport
{
    public sealed class CommitImportService : ICommitImportService
    {
        private readonly IImportBatchRepository _batches;
        private readonly IProcessingJobRepository _jobs;
        private readonly ImportBatchDtoLoader _batchDtos;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public CommitImportService(
            IImportBatchRepository batches,
            IProcessingJobRepository jobs,
            ImportBatchDtoLoader batchDtos,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IClock clock)
        {
            _batches = batches;
            _jobs = jobs;
            _batchDtos = batchDtos;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<BackofficeImportBatchDto> CommitAsync(ICommitImportCommand command, CancellationToken cancellationToken = default)
        {
            var batch = await _batches.GetAsync(command.BatchId, cancellationToken)
                        ?? throw ExceptionFactory.ImportBatchNotFound(command.BatchId);

            var jobId = _idGenerator.NewId();
            var started = batch.StartCommit(command.IdempotencyKey.Trim(), jobId);

            if (started)
            {
                var job = ProcessingJob.Queue(jobId, ProcessingJobKind.IMPORT_COMMIT, batch.Id, batch.Items.Count, _clock.GetDateTime());

                await _jobs.AddAsync(job, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return await _batchDtos.LoadAsync(batch, new Dictionary<long, ShownRows>(), cancellationToken);
        }
    }
}
