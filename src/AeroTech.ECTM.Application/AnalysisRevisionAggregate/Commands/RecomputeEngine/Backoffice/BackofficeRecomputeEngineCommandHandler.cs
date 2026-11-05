using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Commands.RecomputeEngine.Backoffice
{
    public sealed class BackofficeRecomputeEngineCommandHandler : IRequestHandler<BackofficeRecomputeEngineCommand, BackofficeEngineRecomputeDto>
    {
        private readonly IRecomputeEngineService _service;

        public BackofficeRecomputeEngineCommandHandler(IRecomputeEngineService service) => _service = service;

        public Task<BackofficeEngineRecomputeDto> Handle(BackofficeRecomputeEngineCommand command, CancellationToken cancellationToken)
            => _service.RecomputeAsync(command, cancellationToken);
    }
}
