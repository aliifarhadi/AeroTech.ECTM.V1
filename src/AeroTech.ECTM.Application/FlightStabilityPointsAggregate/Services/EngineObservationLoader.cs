using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Services
{
    public sealed class EngineObservationLoader
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;

        public EngineObservationLoader(IFlightRepository flights, IFlightStabilityPointsRepository stabilityPoints)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
        }

        public async Task<EngineObservations> LoadAsync(long engineId, DateOnly? from, DateOnly? to, SnapshotKind kind, string? selector, CancellationToken cancellationToken = default)
        {
            var flights = await _flights.ListWithEngineBetweenAsync(engineId, from, to, cancellationToken);
            var points = await _stabilityPoints.ListCurrentAsync(flights.Select(flight => flight.Id).ToList(), cancellationToken);
            var accepted = points.Where(point => point.Status == ResultStatus.FINAL).ToList();
            var flightById = flights.ToDictionary(flight => flight.Id);
            var observations = new List<SnapshotObservation>();

            foreach (var point in accepted)
            {
                var position = PositionOf(flightById[point.FlightId], engineId);

                if (SnapshotObservation.Of(point.ReadResult(), position, kind, selector) is { } observation)
                    observations.Add(observation);
            }

            return new EngineObservations(flights.Count, accepted.Count, observations);
        }

        private static int PositionOf(Flight flight, long engineId)
            => flight.Engines.First(engine => engine.EngineId == engineId).Position;
    }

    public sealed record EngineObservations(int Flights, int AcceptedFlights, IReadOnlyList<SnapshotObservation> Observations);
}
