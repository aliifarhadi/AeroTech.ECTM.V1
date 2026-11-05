using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeFlightDryRunDto(
        long DocumentId,
        EnumValueDto Kind,
        int VersionNumber,
        EnumValueDto DocumentStatus,
        long FlightId,
        BackofficeConfigurationVersionsDto FlightVersions,
        BackofficeConfigurationVersionsDto DryRunVersions,
        long? ActiveDocumentId,
        int? ActiveVersionNumber,
        IReadOnlyList<BackofficeValidationIssueDto> ConfigurationIssues,
        int SampleCount,
        IReadOnlyList<BackofficeFindingChangeDto> Findings,
        IReadOnlyList<BackofficeDerivedDryRunDto> DerivedParameters,
        IReadOnlyList<string> RemovedFromActive);
}
