using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto
{
    public sealed record BackofficeFindingChangeDto(
        string Change,
        string Code,
        string? ParameterKey,
        EnumValueDto? StoredSeverity,
        long? StoredCount,
        double? StoredValue,
        EnumValueDto? DryRunSeverity,
        long? DryRunCount,
        double? DryRunValue,
        string Message)
    {
        public static BackofficeFindingChangeDto From(FindingChange change)
            => new(
                change.Change,
                change.Code,
                change.ParameterKey,
                EnumValueDto.OfNullable(change.StoredSeverity),
                change.StoredCount,
                change.StoredValue,
                EnumValueDto.OfNullable(change.DryRunSeverity),
                change.DryRunCount,
                change.DryRunValue,
                change.Message);
    }
}
