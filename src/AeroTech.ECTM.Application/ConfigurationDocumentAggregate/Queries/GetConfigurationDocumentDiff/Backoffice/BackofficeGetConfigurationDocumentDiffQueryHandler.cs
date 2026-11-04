using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationDocumentDiff.Backoffice
{
    public sealed class BackofficeGetConfigurationDocumentDiffQueryHandler : IRequestHandler<BackofficeGetConfigurationDocumentDiffQuery, BackofficeConfigurationDiffDto>
    {
        private readonly IGetConfigurationDocumentDiffService _service;

        public BackofficeGetConfigurationDocumentDiffQueryHandler(IGetConfigurationDocumentDiffService service) => _service = service;

        public Task<BackofficeConfigurationDiffDto> Handle(BackofficeGetConfigurationDocumentDiffQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.FromDocumentId, query.ToDocumentId, cancellationToken);
    }
}
