using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ProcessingJobAggregate.Contracts;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById
{
    public sealed class GetProcessingJobByIdService : IGetProcessingJobByIdService
    {
        private readonly IProcessingJobRepository _jobs;

        public GetProcessingJobByIdService(IProcessingJobRepository jobs) => _jobs = jobs;

        public async Task<BackofficeProcessingJobDto> ExecuteAsync(long jobId, CancellationToken cancellationToken = default)
        {
            var job = await _jobs.GetAsync(jobId, cancellationToken)
                      ?? throw ExceptionFactory.ProcessingJobNotFound(jobId);

            return BackofficeProcessingJobDto.From(job);
        }
    }
}
