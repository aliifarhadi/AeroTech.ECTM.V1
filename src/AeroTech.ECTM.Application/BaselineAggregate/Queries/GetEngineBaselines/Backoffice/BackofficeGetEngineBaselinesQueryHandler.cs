using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines.Backoffice
{
    public sealed class BackofficeGetEngineBaselinesQueryHandler : IRequestHandler<BackofficeGetEngineBaselinesQuery, BackofficeEngineBaselinesDto>
    {
        private readonly IGetEngineBaselinesService _service;

        public BackofficeGetEngineBaselinesQueryHandler(IGetEngineBaselinesService service) => _service = service;

        public Task<BackofficeEngineBaselinesDto> Handle(BackofficeGetEngineBaselinesQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineId, cancellationToken);
    }
}
