using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public sealed record ObservedMeasurement(
        FlightSource Source,
        SnapshotKind SnapshotKind,
        string? Selector,
        string ParameterCode,
        string? Unit,
        double? Value,
        int? StartIndex,
        int? EndIndexInclusive,
        int? PeakIndex,
        MeasurementContext Context,
        SnapshotContextStatus? ContextStatus)
    {
        public static ObservedMeasurement Read(FlightSource source, StabilityPointsResult result, BaselineParameter parameter, string? unit)
        {
            var reading = SnapshotReading.Of(result, parameter.SnapshotKind, parameter.Selector, source.Position);

            if (reading is not { IsAvailable: true })
                return Missing(source, parameter, unit);

            return new ObservedMeasurement(
                source,
                reading.Kind,
                reading.Selector,
                parameter.ParameterCode,
                unit,
                reading.MeanOf(parameter.ParameterCode),
                reading.StartIndex,
                reading.EndIndexInclusive,
                reading.PeakIndex,
                MeasurementContext.Of(reading),
                reading.ContextStatus);
        }

        private static ObservedMeasurement Missing(FlightSource source, BaselineParameter parameter, string? unit)
            => new(
                source,
                parameter.SnapshotKind,
                parameter.Selector,
                parameter.ParameterCode,
                unit,
                null,
                null,
                null,
                null,
                MeasurementContext.Unknown,
                null);
    }
}
