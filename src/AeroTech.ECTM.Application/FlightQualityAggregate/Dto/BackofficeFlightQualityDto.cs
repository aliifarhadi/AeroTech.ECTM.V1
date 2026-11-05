using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Dto
{
    public sealed record BackofficeFlightQualityDto(
        long Id,
        long FlightId,
        long QualityRulesDocumentId,
        int QualityRulesVersion,
        EnumValueDto Status,
        EnumValueDto Grade,
        IReadOnlyList<string> GradeReasons,
        int SampleCount,
        IReadOnlyList<int> ElapsedGaps,
        bool? PartialStart,
        bool? PartialEnd,
        int? AirGroundLagSec,
        int? FirstRadioHeightAirborneIndex,
        int? FirstAirFlagIndex,
        IReadOnlyList<Discontinuity> Discontinuities,
        IReadOnlyList<QualitySpan> Spans,
        BackofficeClockEvidenceDto? Clock,
        IReadOnlyList<ChannelCoverage> Coverage,
        IReadOnlyList<BackofficeValidationFindingDto> Findings,
        DateTimeOffset EvaluatedUtc)
    {
        public static BackofficeFlightQualityDto From(FlightQuality quality)
        {
            var result = quality.ReadResult();

            return new BackofficeFlightQualityDto(
                quality.Id,
                quality.FlightId,
                quality.QualityRulesDocumentId,
                quality.QualityRulesVersion,
                EnumValueDto.Of(quality.Status),
                EnumValueDto.Of(quality.Grade),
                result.GradeReasons,
                result.SampleCount,
                result.ElapsedGaps,
                result.PartialStart,
                result.PartialEnd,
                result.AirGroundLagSec,
                result.FirstRadioHeightAirborneIndex,
                result.FirstAirFlagIndex,
                result.Discontinuities,
                result.Spans,
                result.Clock is { } clock ? BackofficeClockEvidenceDto.From(clock) : null,
                result.Coverage,
                result.Findings.Select(BackofficeValidationFindingDto.From).ToList(),
                quality.EvaluatedAt);
        }
    }
}
