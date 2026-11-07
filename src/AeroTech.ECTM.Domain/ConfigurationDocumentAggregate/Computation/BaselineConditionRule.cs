namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record BaselineConditionRule(string? Parameter, bool? PerEngine, double? Tolerance);
}
