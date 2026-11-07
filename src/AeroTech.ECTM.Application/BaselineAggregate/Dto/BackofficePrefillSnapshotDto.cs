using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.BaselineAggregate.Services;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Dto
{
    public sealed record BackofficePrefillSnapshotDto(
        EnumValueDto SnapshotKind,
        string? Selector,
        EnumValueDto Statistic,
        EnumValueDto Availability,
        EnumValueDto? Reason,
        EnumValueDto? ContextStatus,
        BackofficeSourceWindowDto Source,
        IReadOnlyList<BackofficeConditionValueDto> ReferenceConditions,
        string? ReferenceConfiguration,
        IReadOnlyList<BackofficePrefillValueDto> Values)
    {
        public static BackofficePrefillSnapshotDto From(SnapshotReading reading, BaselineDefinitions definitions)
        {
            var context = MeasurementContext.Of(reading);
            var values = definitions.EngineParameters
                .Select(code => ValueOf(reading, code, definitions))
                .ToList();

            return new BackofficePrefillSnapshotDto(
                EnumValueDto.Of(reading.Kind),
                reading.Selector,
                EnumValueDto.Of(MeasurementStatistic.Of(reading.Kind)),
                EnumValueDto.Of(reading.Availability),
                EnumValueDto.OfNullable(reading.Reason),
                EnumValueDto.OfNullable(reading.ContextStatus),
                new BackofficeSourceWindowDto(reading.Position, reading.StartIndex, reading.EndIndexInclusive, reading.PeakIndex),
                BackofficeConditionValueDto.ListOf(context),
                context.Configuration,
                values);
        }

        private static BackofficePrefillValueDto ValueOf(SnapshotReading reading, string parameterCode, BaselineDefinitions definitions)
        {
            var key = SnapshotFrame.KeyOf(parameterCode, reading.Position);
            var summary = reading.Values.FirstOrDefault(value => value.ParameterKey == key);
            var suggested = reading.IsAvailable ? summary?.Mean : null;

            return new BackofficePrefillValueDto(parameterCode, definitions.UnitOf(parameterCode), suggested, summary?.Count ?? 0);
        }
    }
}
