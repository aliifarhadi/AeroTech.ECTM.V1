namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record BaselineRulesValidation(BaselineRulesContent? Rules, IReadOnlyList<ValidationIssue> Issues)
    {
        public bool IsValid => Issues.Count == 0;
    }
}
