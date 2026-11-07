using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficeBaselinePreviewDto(
        BackofficeBaselineDto Baseline,
        bool CanPublish,
        IReadOnlyList<EnumValueDto> Issues,
        long? PreviewFlightId,
        string? PreviewFlightNumber,
        DateOnly? PreviewFlightDate,
        IReadOnlyList<BackofficeBaselineEvaluationDto> Evaluations);
}
