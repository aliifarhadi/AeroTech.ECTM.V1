using System.Globalization;
using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto
{
    public sealed record BackofficeBaselineEvaluationDto(
        long? Id,
        long BaselineId,
        string BaselineName,
        int BaselineRevisionNumber,
        EnumValueDto BaselineState,
        long BaselineParameterId,
        bool IsPrimary,
        long FlightId,
        string? FlightNumber,
        DateOnly? FlightDate,
        long EngineId,
        int Position,
        long AnalysisRevisionId,
        long FlightStabilityPointsId,
        long SnapshotRulesDocumentId,
        int SnapshotRulesVersion,
        EnumValueDto SnapshotKind,
        string? Selector,
        EnumValueDto Statistic,
        string ParameterCode,
        string? Unit,
        double? RawObserved,
        double BaselineRawValue,
        double? RawDelta,
        double? CorrectedObserved,
        double? CorrectedBaseline,
        double? CorrectedDelta,
        EnumValueDto Status,
        IReadOnlyList<EnumValueDto> Reasons,
        IReadOnlyList<BackofficeConditionCheckDto> Conditions,
        string? ObservedConfiguration,
        string? ReferenceConfiguration,
        BackofficeSourceWindowDto Source,
        string? Arithmetic,
        bool IsCurrent,
        DateTimeOffset CalculatedAt)
    {
        public static BackofficeBaselineEvaluationDto From(
            BaselineParameterFlight evaluation,
            long? id,
            Baseline baseline,
            string? flightNumber,
            bool isPrimary,
            ComparisonStatus status)
        {
            var detail = evaluation.ReadDetail();
            var source = new BackofficeSourceWindowDto(
                evaluation.Position,
                evaluation.SourceStartIndex,
                evaluation.SourceEndIndexInclusive,
                evaluation.SourcePeakIndex);

            return new BackofficeBaselineEvaluationDto(
                id,
                baseline.Id,
                baseline.Name,
                baseline.RevisionNumber,
                EnumValueDto.Of(baseline.State),
                evaluation.BaselineParameterId,
                isPrimary,
                evaluation.FlightId,
                flightNumber,
                evaluation.FlightDate,
                evaluation.EngineId,
                evaluation.Position,
                evaluation.AnalysisRevisionId,
                evaluation.FlightStabilityPointsId,
                evaluation.SnapshotRulesDocumentId,
                evaluation.SnapshotRulesVersion,
                EnumValueDto.Of(evaluation.SnapshotKind),
                evaluation.Selector,
                EnumValueDto.Of(evaluation.Statistic),
                evaluation.ParameterCode,
                evaluation.Unit,
                evaluation.RawObserved,
                evaluation.BaselineRawValue,
                evaluation.RawDelta,
                null,
                null,
                null,
                EnumValueDto.Of(status),
                evaluation.Reasons.Select(reason => EnumValueDto.Of(reason)).ToList(),
                detail.Conditions.Select(BackofficeConditionCheckDto.From).ToList(),
                detail.ObservedConfiguration,
                detail.ReferenceConfiguration,
                source,
                ArithmeticOf(evaluation),
                evaluation.IsCurrent,
                evaluation.CalculatedAt);
        }

        private static string? ArithmeticOf(BaselineParameterFlight evaluation)
        {
            if (evaluation.RawObserved is not { } observed || evaluation.RawDelta is not { } delta)
                return null;

            var reference = evaluation.BaselineRawValue;
            var sign = delta >= 0 ? "+" : string.Empty;

            return string.Create(CultureInfo.InvariantCulture, $"{observed:R} - {reference:R} = {sign}{delta:R} {evaluation.Unit}").TrimEnd();
        }
    }
}
