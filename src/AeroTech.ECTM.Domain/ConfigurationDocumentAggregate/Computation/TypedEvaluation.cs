namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record TypedEvaluation(
        string Code,
        string Kind,
        double? Value,
        string? Unit,
        string Status,
        IReadOnlyList<string> Reasons,
        IReadOnlyList<EvaluatedInput> Inputs,
        IReadOnlyDictionary<string, double> Constants,
        string? Formula);
}
