using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeValidationIssueDto(string Path, string Code, string Message)
    {
        public static BackofficeValidationIssueDto From(ValidationIssue issue) => new(issue.Path, issue.Code, issue.Message);
    }
}
