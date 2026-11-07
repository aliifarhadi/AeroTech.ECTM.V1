using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeBaselineParameterDto(
        long Id,
        EnumValueDto SnapshotKind,
        string? Selector,
        EnumValueDto Statistic,
        string ParameterCode,
        string? Unit,
        double? RawValue,
        double? SuggestedRawValue,
        EnumValueDto ValueOrigin,
        bool IsPrimary,
        BackofficeSourceWindowDto Source,
        IReadOnlyList<BackofficeConditionValueDto> ReferenceConditions,
        string? ReferenceConfiguration,
        long CatalogDocumentId,
        int CatalogVersion,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        string? Notes)
    {
        public static BackofficeBaselineParameterDto From(BaselineParameter parameter, bool isPrimary)
        {
            var referenceContext = parameter.ReadReferenceContext();
            var source = new BackofficeSourceWindowDto(
                parameter.SourceEnginePosition,
                parameter.SourceStartIndex,
                parameter.SourceEndIndexInclusive,
                parameter.SourcePeakIndex);

            return new BackofficeBaselineParameterDto(
                parameter.Id,
                EnumValueDto.Of(parameter.SnapshotKind),
                parameter.Selector,
                EnumValueDto.Of(parameter.Statistic),
                parameter.ParameterCode,
                parameter.Unit,
                parameter.RawValue,
                parameter.SuggestedRawValue,
                EnumValueDto.Of(parameter.ValueOrigin),
                isPrimary,
                source,
                BackofficeConditionValueDto.ListOf(referenceContext),
                referenceContext.Configuration,
                parameter.CatalogDocumentId,
                parameter.CatalogVersion,
                parameter.SnapshotRulesDocumentId,
                parameter.SnapshotRulesVersion,
                parameter.Notes);
        }
    }
}
