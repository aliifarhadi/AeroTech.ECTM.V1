namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ValidationIssue(string Path, string Code, string Message);
}
