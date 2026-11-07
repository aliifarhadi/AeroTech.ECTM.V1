using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;

using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline.Backoffice
{
    public sealed class BackofficeReconcileBaselineCommandHandler : IRequestHandler<BackofficeReconcileBaselineCommand, BackofficeProcessingJobDto>
    {
        private readonly IReconcileBaselineService _service;

        public BackofficeReconcileBaselineCommandHandler(IReconcileBaselineService service) => _service = service;

        public Task<BackofficeProcessingJobDto> Handle(BackofficeReconcileBaselineCommand command, CancellationToken cancellationToken)
            => _service.ReconcileAsync(command, cancellationToken);
    }
}
