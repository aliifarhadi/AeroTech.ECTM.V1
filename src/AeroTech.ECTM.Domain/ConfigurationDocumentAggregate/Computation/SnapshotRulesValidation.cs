namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record SnapshotRulesValidation(SnapshotRulesContent? Rules, IReadOnlyList<ValidationIssue> Issues, IReadOnlySet<string> ParameterCodes)
    {
        public bool IsValid => Issues.Count == 0;
    }
}
