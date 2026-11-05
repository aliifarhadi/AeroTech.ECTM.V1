namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Services
{
    public interface IProcessingJobRunner
    {
        Task<bool> RunNextAsync(CancellationToken cancellationToken = default);
    }
}
