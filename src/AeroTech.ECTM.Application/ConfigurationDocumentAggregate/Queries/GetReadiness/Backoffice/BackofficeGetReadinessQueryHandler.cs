using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness.Backoffice
{
    public sealed class BackofficeGetReadinessQueryHandler : IRequestHandler<BackofficeGetReadinessQuery, BackofficeReadinessDto>
    {
        private readonly IGetReadinessService _service;

        public BackofficeGetReadinessQueryHandler(IGetReadinessService service) => _service = service;

        public Task<BackofficeReadinessDto> Handle(BackofficeGetReadinessQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(cancellationToken);
    }
}
