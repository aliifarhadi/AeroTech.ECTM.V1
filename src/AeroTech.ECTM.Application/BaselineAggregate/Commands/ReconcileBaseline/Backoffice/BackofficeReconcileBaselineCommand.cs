using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline.Backoffice
{
    public sealed record BackofficeReconcileBaselineCommand(
        long BaselineId) : IRequest<BackofficeProcessingJobDto>, IReconcileBaselineCommand;
}
