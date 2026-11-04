using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition
{
    public interface IEvaluateTypedDefinitionService
    {
        Task<BackofficeTypedEvaluationDto> ExecuteAsync(IEvaluateTypedDefinitionQuery query, CancellationToken cancellationToken = default);
    }
}
