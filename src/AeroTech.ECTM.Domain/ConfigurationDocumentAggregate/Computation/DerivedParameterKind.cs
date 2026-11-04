namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record DerivedParameterKind(string Code, IReadOnlyList<string> Fields, string AvailableFrom)
    {
        public bool IsAvailableNow => AvailableFrom == DerivedParameterKinds.AvailableNow;
    }
}
