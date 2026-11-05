using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById
{
    public interface IGetProcessingJobByIdService
    {
        Task<BackofficeProcessingJobDto> ExecuteAsync(long jobId, CancellationToken cancellationToken = default);
    }
}
