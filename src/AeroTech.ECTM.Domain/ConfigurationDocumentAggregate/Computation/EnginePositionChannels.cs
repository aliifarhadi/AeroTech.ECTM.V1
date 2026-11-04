namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record EnginePositionChannels(int Position, IReadOnlyList<string> ParameterKeys);
}
