namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record ProfileValidation(DataProfileContent? Profile, IReadOnlyList<ValidationIssue> Issues)
    {
        public bool IsValid => Issues.Count == 0;
    }
}
