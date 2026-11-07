using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline
{
    public interface IReconcileBaselineService
    {
        Task<BackofficeProcessingJobDto> ReconcileAsync(IReconcileBaselineCommand command, CancellationToken cancellationToken = default);
    }
}
