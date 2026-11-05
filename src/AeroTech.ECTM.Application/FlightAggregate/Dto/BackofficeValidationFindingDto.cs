using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;

namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeValidationFindingDto(
        EnumValueDto Severity,
        string Code,
        string Message,
        string? ParameterKey,
        long? Count,
        double? Value)
    {
        public static BackofficeValidationFindingDto From(ValidationFinding finding)
            => new(EnumValueDto.Of(finding.Severity), finding.Code, finding.Message, finding.ParameterKey, finding.Count, finding.Value);

        public static BackofficeValidationFindingDto From(FlightValidationIssue issue)
            => new(EnumValueDto.Of(issue.Severity), issue.Code, issue.Message, issue.ParameterKey, issue.Count, issue.Value);
    }
}
