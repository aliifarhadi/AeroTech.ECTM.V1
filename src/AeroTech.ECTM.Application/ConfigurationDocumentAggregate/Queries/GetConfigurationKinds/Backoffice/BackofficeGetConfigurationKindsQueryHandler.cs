using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds.Backoffice
{
    public sealed class BackofficeGetConfigurationKindsQueryHandler : IRequestHandler<BackofficeGetConfigurationKindsQuery, IReadOnlyList<BackofficeConfigurationKindDto>>
    {
        private readonly IGetConfigurationKindsService _service;

        public BackofficeGetConfigurationKindsQueryHandler(IGetConfigurationKindsService service) => _service = service;

        public Task<IReadOnlyList<BackofficeConfigurationKindDto>> Handle(BackofficeGetConfigurationKindsQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(cancellationToken);
    }
}
