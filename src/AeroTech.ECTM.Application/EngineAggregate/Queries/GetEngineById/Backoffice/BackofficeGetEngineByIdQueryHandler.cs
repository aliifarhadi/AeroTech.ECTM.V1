using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById.Backoffice
{
    public sealed class BackofficeGetEngineByIdQueryHandler : IRequestHandler<BackofficeGetEngineByIdQuery, BackofficeEngineDetailDto>
    {
        private readonly IGetEngineByIdService _service;

        public BackofficeGetEngineByIdQueryHandler(IGetEngineByIdService service) => _service = service;

        public Task<BackofficeEngineDetailDto> Handle(BackofficeGetEngineByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.EngineId, cancellationToken);
    }
}
