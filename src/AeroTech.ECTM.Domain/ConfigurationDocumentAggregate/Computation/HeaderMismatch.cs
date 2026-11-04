namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record HeaderMismatch(int Ordinal, string? ExpectedHeader, string? ActualHeader);
}
