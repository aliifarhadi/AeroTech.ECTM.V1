namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record JsonChange(string Path, string Change, string? Before, string? After);
}
