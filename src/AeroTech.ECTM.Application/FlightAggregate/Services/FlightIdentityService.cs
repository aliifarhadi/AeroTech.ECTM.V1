using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.EngineInstallationAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightAggregate.Services
{
    public sealed class FlightIdentityService
    {
        private readonly IAircraftRepository _aircraft;
        private readonly IEngineInstallationRepository _installations;
        private readonly IFlightRepository _flights;
        private readonly Dictionary<string, Aircraft?> _aircraftByRegistration = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<long, IReadOnlyList<EngineInstallation>> _installationsByAircraft = new();

        public FlightIdentityService(IAircraftRepository aircraft, IEngineInstallationRepository installations, IFlightRepository flights)
        {
            _aircraft = aircraft;
            _installations = installations;
            _flights = flights;
        }

        public async Task<IdentityResolution> ResolveAsync(
            IdentityClaim claim,
            IReadOnlyList<int> positions,
            long excludedFlightId,
            bool duplicateInBatch,
            CancellationToken cancellationToken = default)
        {
            var aircraft = await AircraftOfAsync(claim.Registration, cancellationToken);
            IReadOnlyList<EngineInstallation> installations = [];
            var possibleDuplicate = duplicateInBatch;

            if (aircraft is not null)
                installations = await InstallationsOfAsync(aircraft.Id, cancellationToken);

            if (aircraft is not null && claim.FlightDate is { } flightDate && claim.FlightNumber is { } flightNumber)
            {
                var sameIdentity = await _flights.FindSameIdentityAsync(excludedFlightId, aircraft.Id, flightDate, flightNumber, cancellationToken);
                possibleDuplicate = possibleDuplicate || sameIdentity is not null;
            }

            return IdentityResolver.Resolve(claim, aircraft, installations, positions, possibleDuplicate);
        }

        public async Task<IReadOnlyList<EngineInstallation>> InstallationsOfAsync(long aircraftId, CancellationToken cancellationToken = default)
        {
            if (_installationsByAircraft.TryGetValue(aircraftId, out var cached))
                return cached;

            var installations = await _installations.ListByAircraftIdsAsync([aircraftId], cancellationToken);
            _installationsByAircraft[aircraftId] = installations;

            return installations;
        }

        private async Task<Aircraft?> AircraftOfAsync(string? registration, CancellationToken cancellationToken)
        {
            if (registration is null)
                return null;

            if (_aircraftByRegistration.TryGetValue(registration, out var cached))
                return cached;

            var matches = await _aircraft.ListByRegistrationsAsync([registration], cancellationToken);
            var aircraft = matches.FirstOrDefault();
            _aircraftByRegistration[registration] = aircraft;

            return aircraft;
        }
    }
}
