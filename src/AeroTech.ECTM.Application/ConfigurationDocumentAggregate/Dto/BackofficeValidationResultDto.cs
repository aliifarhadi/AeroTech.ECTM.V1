using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeValidationResultDto(bool Valid, IReadOnlyList<BackofficeValidationIssueDto> Issues)
    {
        public static BackofficeValidationResultDto From(IReadOnlyList<ValidationIssue> issues)
            => new(issues.Count == 0, issues.Select(BackofficeValidationIssueDto.From).ToList());
    }
}
