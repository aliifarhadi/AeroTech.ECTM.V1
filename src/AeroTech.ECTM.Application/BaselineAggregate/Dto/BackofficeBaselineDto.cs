using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.BaselineAggregate;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeBaselineDto(
        long Id,
        long EngineId,
        string? EngineSerialNumber,
        string Name,
        long LineageId,
        int RevisionNumber,
        long? PreviousBaselineId,
        EnumValueDto State,
        int DraftRevision,
        EnumValueDto InputProvenance,
        DateOnly? DeclaredEffectiveDate,
        long? ShopVisitEventId,
        bool FirstPostShopDeclared,
        string? SourceFileName,
        string? SourceFileSha256,
        string? Notes,
        IReadOnlyList<EnumValueDto> PublicationIssues,
        IReadOnlyList<BackofficeBaselineParameterDto> Parameters,
        string? CreatedBy,
        DateTimeOffset CreatedAt,
        string? PublishedBy,
        DateTimeOffset? PublishedAt,
        string? RetiredBy,
        DateTimeOffset? RetiredAt,
        string? RetirementReason)
    {
        public static BackofficeBaselineDto From(Baseline baseline, string? engineSerialNumber, IReadOnlySet<long> primaryParameterIds)
        {
            var parameters = baseline.Parameters
                .OrderBy(parameter => parameter.SnapshotKind)
                .ThenBy(parameter => parameter.Selector)
                .ThenBy(parameter => parameter.ParameterCode)
                .Select(parameter => BackofficeBaselineParameterDto.From(parameter, primaryParameterIds.Contains(parameter.Id)))
                .ToList();

            return new BackofficeBaselineDto(
                baseline.Id,
                baseline.EngineId,
                engineSerialNumber,
                baseline.Name,
                baseline.LineageId,
                baseline.RevisionNumber,
                baseline.PreviousBaselineId,
                EnumValueDto.Of(baseline.State),
                baseline.DraftRevision,
                EnumValueDto.Of(baseline.InputProvenance),
                baseline.DeclaredEffectiveDate,
                baseline.ShopVisitEventId,
                baseline.FirstPostShopDeclared,
                baseline.SourceFileName,
                baseline.SourceFileSha256,
                baseline.Notes,
                baseline.PublicationIssues().Select(issue => EnumValueDto.Of(issue)).ToList(),
                parameters,
                baseline.CreatedBy,
                baseline.CreatedAt,
                baseline.PublishedBy,
                baseline.PublishedAt,
                baseline.RetiredBy,
                baseline.RetiredAt,
                baseline.RetirementReason);
        }
    }
}
