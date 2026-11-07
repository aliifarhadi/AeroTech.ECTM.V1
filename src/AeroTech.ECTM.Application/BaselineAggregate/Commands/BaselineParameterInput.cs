using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands
{
    public sealed record BaselineParameterInput(
        SnapshotKind SnapshotKind,
        string? Selector,
        string ParameterCode,
        double? RawValue,
        double? SuggestedRawValue,
        int? SourceEnginePosition,
        int? SourceStartIndex,
        int? SourceEndIndexInclusive,
        int? SourcePeakIndex,
        IReadOnlyList<ConditionValueInput>? ReferenceConditions,
        string? ReferenceConfiguration,
        string? Notes);
}
