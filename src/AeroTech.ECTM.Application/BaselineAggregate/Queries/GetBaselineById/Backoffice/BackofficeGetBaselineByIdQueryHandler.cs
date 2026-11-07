using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById.Backoffice
{
    public sealed class BackofficeGetBaselineByIdQueryHandler : IRequestHandler<BackofficeGetBaselineByIdQuery, BackofficeBaselineDto>
    {
        private readonly IGetBaselineByIdService _service;

        public BackofficeGetBaselineByIdQueryHandler(IGetBaselineByIdService service) => _service = service;

        public Task<BackofficeBaselineDto> Handle(BackofficeGetBaselineByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.BaselineId, cancellationToken);
    }
}
