using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition.Backoffice
{
    public sealed class BackofficeEvaluateTypedDefinitionQueryHandler : IRequestHandler<BackofficeEvaluateTypedDefinitionQuery, BackofficeTypedEvaluationDto>
    {
        private readonly IEvaluateTypedDefinitionService _service;

        public BackofficeEvaluateTypedDefinitionQueryHandler(IEvaluateTypedDefinitionService service) => _service = service;

        public Task<BackofficeTypedEvaluationDto> Handle(BackofficeEvaluateTypedDefinitionQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query, cancellationToken);
    }
}
