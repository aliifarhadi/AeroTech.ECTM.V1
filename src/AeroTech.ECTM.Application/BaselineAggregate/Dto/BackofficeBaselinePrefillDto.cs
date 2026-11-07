using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeBaselinePrefillDto(
        long EngineId,
        string? EngineSerialNumber,
        int EnginePosition,
        long? ShopVisitEventId,
        string SourceFileName,
        string SourceFileSha256,
        bool FileArchived,
        bool FirstPostShopVerified,
        EnumValueDto SourceQuality,
        EnumValueDto? NotEvaluableReason,
        long CatalogDocumentId,
        int CatalogVersion,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        IReadOnlyList<BackofficePrefillSnapshotDto> Snapshots);
}
