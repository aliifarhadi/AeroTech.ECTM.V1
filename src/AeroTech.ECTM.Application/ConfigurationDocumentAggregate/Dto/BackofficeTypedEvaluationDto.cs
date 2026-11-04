using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeTypedEvaluationDto(
        long CatalogDocumentId,
        int CatalogVersionNumber,
        string Code,
        string Kind,
        double? Value,
        string? Unit,
        string Status,
        IReadOnlyList<string> Reasons,
        IReadOnlyList<EvaluatedInput> Inputs,
        IReadOnlyDictionary<string, double> Constants,
        string? Formula)
    {
        public static BackofficeTypedEvaluationDto From(long catalogDocumentId, int catalogVersionNumber, TypedEvaluation evaluation)
            => new(
                catalogDocumentId,
                catalogVersionNumber,
                evaluation.Code,
                evaluation.Kind,
                evaluation.Value,
                evaluation.Unit,
                evaluation.Status,
                evaluation.Reasons,
                evaluation.Inputs,
                evaluation.Constants,
                evaluation.Formula);
    }
}
