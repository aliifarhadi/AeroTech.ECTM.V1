using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition
{
    public sealed record EvaluationInput(double? Value, string? Unit)
    {
        public TypedInput ToArgs() => new(Value, Unit);
    }
}
