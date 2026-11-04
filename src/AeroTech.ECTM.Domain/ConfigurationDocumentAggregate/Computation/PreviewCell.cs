namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record PreviewCell(int Ordinal, string? ParameterKey, string Raw, double? Number, string Status);
}
