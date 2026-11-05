using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class IdentityResolver
    {
        public static IdentityResolution Resolve(
            IdentityClaim claim,
            Aircraft? aircraft,
            IReadOnlyList<EngineInstallation> installations,
            IReadOnlyList<int> positions,
            bool possibleDuplicate)
        {
            var reasons = claim.Reasons.ToList();

            if (claim.Registration is not null && aircraft is null)
                reasons.Add(IdentityReasons.AircraftNotFound);

            var engines = new List<EngineResolution>();

            foreach (var position in positions)
                engines.Add(EngineAt(aircraft, claim.FlightDate, position, installations, reasons));

            if (possibleDuplicate)
                reasons.Add(IdentityReasons.PossibleDuplicateIdentity);

            return new IdentityResolution(
                aircraft?.Id,
                aircraft?.Registration ?? claim.Registration,
                claim.RegistrationSource,
                claim.FlightDate,
                claim.FlightDateSource,
                claim.FlightNumber,
                claim.FlightNumberSource,
                StatusOf(reasons),
                reasons,
                engines);
        }

        public static IdentityResolution ConfirmByUser(
            Aircraft aircraft,
            DateOnly flightDate,
            string flightNumber,
            IReadOnlyList<EngineChoiceArgs> choices,
            IReadOnlyList<int> positions,
            IReadOnlyList<EngineInstallation> installations)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw ExceptionFactory.FlightIdentityIsIncomplete("a flight number is required");

            var engines = new List<EngineResolution>();

            foreach (var position in positions)
            {
                var positionChoices = choices.Where(choice => choice.Position == position).ToList();

                if (positionChoices.Count != 1)
                    throw ExceptionFactory.FlightIdentityIsIncomplete($"position {position} needs exactly one choice");

                engines.Add(ConfirmedEngine(aircraft, flightDate, positionChoices[0], installations));
            }

            var unknownPosition = choices.FirstOrDefault(choice => !positions.Contains(choice.Position));

            if (unknownPosition is not null)
                throw ExceptionFactory.FlightIdentityIsIncomplete($"position {unknownPosition.Position} is not recorded in this flight");

            return new IdentityResolution(
                aircraft.Id,
                aircraft.Registration,
                IdentitySource.USER,
                flightDate,
                IdentitySource.USER,
                flightNumber.Trim().ToUpperInvariant(),
                IdentitySource.USER,
                IdentityStatus.RESOLVED,
                [],
                engines);
        }

        public static IReadOnlyList<EngineInstallation> InstalledDuring(IReadOnlyList<EngineInstallation> installations, long aircraftId, int position, DateOnly flightDate)
        {
            var (dayStart, dayEnd) = DayOf(flightDate);

            return installations
                .Where(installation => installation.AircraftId == aircraftId)
                .Where(installation => installation.Position == position)
                .Where(installation => installation.InstalledAt < dayEnd)
                .Where(installation => (installation.RemovedAt ?? DateTimeOffset.MaxValue) > dayStart)
                .ToList();
        }

        private static EngineResolution EngineAt(Aircraft? aircraft, DateOnly? flightDate, int position, IReadOnlyList<EngineInstallation> installations, List<string> reasons)
        {
            if (aircraft is null || flightDate is not { } date)
                return new EngineResolution(position, EngineAssignmentStatus.NEEDS_CONFIRMATION, null, null);

            var candidates = InstalledDuring(installations, aircraft.Id, position, date);

            if (candidates.Count == 0)
            {
                reasons.Add(IdentityReasons.NoInstallation(position));
                return new EngineResolution(position, EngineAssignmentStatus.POSITION_ONLY, null, null);
            }

            var (dayStart, dayEnd) = DayOf(date);
            var installation = candidates[0];
            var coversWholeDay = installation.InstalledAt <= dayStart && (installation.RemovedAt ?? DateTimeOffset.MaxValue) >= dayEnd;

            if (candidates.Count == 1 && coversWholeDay)
                return new EngineResolution(position, EngineAssignmentStatus.ENGINE, installation.EngineId, installation.Id);

            reasons.Add(IdentityReasons.InstallationChanged(position));

            return new EngineResolution(position, EngineAssignmentStatus.NEEDS_CONFIRMATION, null, null);
        }

        private static EngineResolution ConfirmedEngine(Aircraft aircraft, DateOnly flightDate, EngineChoiceArgs choice, IReadOnlyList<EngineInstallation> installations)
        {
            if (choice.PositionOnly && choice.EngineId is not null)
                throw ExceptionFactory.FlightIdentityIsIncomplete($"position {choice.Position} is either an engine or position only");

            if (choice.PositionOnly)
                return new EngineResolution(choice.Position, EngineAssignmentStatus.POSITION_ONLY, null, null);

            if (choice.EngineId is not { } engineId)
                throw ExceptionFactory.FlightIdentityIsIncomplete($"position {choice.Position} needs an engine or position only");

            var installation = InstalledDuring(installations, aircraft.Id, choice.Position, flightDate)
                .FirstOrDefault(candidate => candidate.EngineId == engineId);

            if (installation is null)
                throw ExceptionFactory.EngineNotInstalledAtFlight(engineId, aircraft.Registration, choice.Position, flightDate.ToString("yyyy-MM-dd"));

            return new EngineResolution(choice.Position, EngineAssignmentStatus.ENGINE, engineId, installation.Id);
        }

        private static IdentityStatus StatusOf(IReadOnlyList<string> reasons)
        {
            if (reasons.Any(IdentityReasons.Unresolved.Contains))
                return IdentityStatus.UNRESOLVED;

            if (reasons.Count > 0)
                return IdentityStatus.NEEDS_CONFIRMATION;

            return IdentityStatus.RESOLVED;
        }

        private static (DateTimeOffset Start, DateTimeOffset End) DayOf(DateOnly date)
        {
            var start = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

            return (start, start.AddDays(1));
        }
    }
}
