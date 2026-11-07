using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineAggregate.Arguments
{
    public sealed record BaselineParameterArgs(
        SnapshotKind SnapshotKind,
        string? Selector,
        string ParameterCode,
        string? Unit,
        double? RawValue,
        double? SuggestedRawValue,
        int? SourceEnginePosition,
        int? SourceStartIndex,
        int? SourceEndIndexInclusive,
        int? SourcePeakIndex,
        MeasurementContext ReferenceContext,
        long CatalogDocumentId,
        int CatalogVersion,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        string? Notes);
}
