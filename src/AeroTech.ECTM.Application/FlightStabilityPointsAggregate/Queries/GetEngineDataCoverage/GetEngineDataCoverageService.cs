using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage
{
    public sealed class GetEngineDataCoverageService : IGetEngineDataCoverageService
    {
        private const double FullCoverage = 100;

        private readonly IEngineRepository _engines;
        private readonly IFlightRepository _flights;
        private readonly IFlightQualityRepository _qualities;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public GetEngineDataCoverageService(
            IEngineRepository engines,
            IFlightRepository flights,
            IFlightQualityRepository qualities,
            IFlightStabilityPointsRepository stabilityPoints)
        {
            _engines = engines;
            _flights = flights;
            _qualities = qualities;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<BackofficeEngineDataCoverageDto> ExecuteAsync(long engineId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var engine = await _engines.GetAsync(engineId, cancellationToken)
                         ?? throw ExceptionFactory.EngineNotFound(engineId);
            var flights = await _flights.ListWithEngineBetweenAsync(engine.Id, from, to, cancellationToken);
            var flightIds = flights.Select(flight => flight.Id).ToList();
            var positionByFlight = flights.ToDictionary(flight => flight.Id, flight => PositionOf(flight, engine.Id));
            var qualities = await _qualities.ListCurrentAsync(flightIds, cancellationToken);
            var points = await _stabilityPoints.ListCurrentAsync(flightIds, cancellationToken);
            var pointResults = points.Select(point => (Position: positionByFlight[point.FlightId], Result: point.ReadResult())).ToList();
            var coverages = qualities.Select(quality => (Position: positionByFlight[quality.FlightId], Coverage: quality.ReadResult().Coverage)).ToList();

            return new BackofficeEngineDataCoverageDto(
                engine.Id,
                engine.SerialNumber,
                from,
                to,
                flights.Count,
                CountsOf(flights.Select(flight => flight.State)),
                CountsOf(qualities.Select(quality => quality.Grade)),
                AvailabilityOf(pointResults),
                MissingChannelsOf(coverages));
        }

        private static int PositionOf(Flight flight, long engineId) => flight.Engines.First(engine => engine.EngineId == engineId).Position;

        private static List<BackofficeStateCountDto> CountsOf<TEnum>(IEnumerable<TEnum> values)
            where TEnum : struct, Enum
            => values
                .GroupBy(value => value)
                .OrderBy(group => group.Key)
                .Select(group => new BackofficeStateCountDto(EnumValueDto.Of(group.Key), group.Count()))
                .ToList();

        private static List<BackofficeAvailabilityCountDto> AvailabilityOf(IReadOnlyList<(int Position, StabilityPointsResult Result)> results)
        {
            var counts = new List<BackofficeAvailabilityCountDto>
            {
                CountOf(SnapshotKind.TAKEOFF_N1_PEAK_CONTEXT, null, results.Select(item => item.Result.TakeoffN1Peaks.FirstOrDefault(peak => peak.Position == item.Position)).Select(peak => (peak?.Availability, peak?.Reason))),
                CountOf(SnapshotKind.TAKEOFF_EGT_PEAK_OBSERVED, null, results.Select(item => item.Result.TakeoffEgtPeaks.FirstOrDefault(peak => peak.Position == item.Position)).Select(peak => (peak?.Availability, peak?.Reason)))
            };
            var selectors = results.SelectMany(item => item.Result.Cruise.Select(selector => selector.Selector)).Distinct();

            foreach (var selector in selectors)
            {
                var engines = results.Select(item => item.Result.Cruise
                    .Where(entry => entry.Selector == selector)
                    .SelectMany(entry => entry.Engines)
                    .FirstOrDefault(engine => engine.Position == item.Position));

                counts.Add(CountOf(SnapshotKind.CRUISE, selector, engines.Select(engine => (engine?.Availability, engine?.Reason))));
            }

            return counts;
        }

        private static BackofficeAvailabilityCountDto CountOf(SnapshotKind kind, string? selector, IEnumerable<(SnapshotAvailability? Availability, SnapshotReason? Reason)> outcomes)
        {
            var list = outcomes.ToList();
            var available = list.Count(outcome => outcome.Availability == SnapshotAvailability.AVAILABLE);
            var reasons = list
                .Where(outcome => outcome.Availability != SnapshotAvailability.AVAILABLE)
                .GroupBy(outcome => outcome.Reason?.ToString() ?? SnapshotReason.NO_PHASE_RESULT.ToString())
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.Count());

            return new BackofficeAvailabilityCountDto(EnumValueDto.Of(kind), selector, available, reasons);
        }

        private static List<BackofficeMissingChannelDto> MissingChannelsOf(IReadOnlyList<(int Position, IReadOnlyList<Domain.FlightQualityAggregate.Computation.ChannelCoverage> Coverage)> coverages)
            => coverages
                .SelectMany(item => item.Coverage.Where(channel => channel.ParameterKey.EndsWith($"[{item.Position}]", StringComparison.Ordinal)))
                .Where(channel => !channel.Recorded || channel.Percent < FullCoverage)
                .GroupBy(channel => channel.ParameterKey)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new BackofficeMissingChannelDto(group.Key, group.Count(channel => !channel.Recorded), group.Count(channel => channel.Recorded)))
                .ToList();
    }
}
