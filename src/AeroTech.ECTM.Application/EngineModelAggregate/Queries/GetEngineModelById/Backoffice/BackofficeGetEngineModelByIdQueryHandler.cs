using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById.Backoffice
{
    public sealed class BackofficeGetEngineModelByIdQueryHandler : IRequestHandler<BackofficeGetEngineModelByIdQuery, BackofficeEngineModelDto>
    {
        private readonly IGetEngineModelByIdService _service;

        public BackofficeGetEngineModelByIdQueryHandler(IGetEngineModelByIdService service) => _service = service;

        public Task<BackofficeEngineModelDto> Handle(BackofficeGetEngineModelByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineModelId, cancellationToken);
    }
}
