namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class IdentityReasons
    {
        public const string RegistrationMissing = "REGISTRATION_MISSING";
        public const string AircraftNotFound = "AIRCRAFT_NOT_FOUND";
        public const string FlightDateMissing = "FLIGHT_DATE_MISSING";
        public const string FlightNumberMissing = "FLIGHT_NUMBER_MISSING";
        public const string FlightNumberMismatch = "FLIGHT_NUMBER_MISMATCH";
        public const string FlightNumberNotConstant = "FLIGHT_NUMBER_NOT_CONSTANT";
        public const string DayMismatch = "DAY_MISMATCH";
        public const string DayNotConstant = "DAY_NOT_CONSTANT";
        public const string MonthMismatch = "MONTH_MISMATCH";
        public const string MonthNotConstant = "MONTH_NOT_CONSTANT";
        public const string PossibleDuplicateIdentity = "POSSIBLE_DUPLICATE_IDENTITY";
        public const string IdentityReviewRequested = "IDENTITY_REVIEW_REQUESTED";

        public static readonly IReadOnlySet<string> Unresolved = new HashSet<string>(StringComparer.Ordinal)
        {
            RegistrationMissing,
            AircraftNotFound,
            FlightDateMissing,
            FlightNumberMissing
        };

        public static string NoInstallation(int position) => $"POSITION_{position}_NO_INSTALLATION";

        public static string InstallationChanged(int position) => $"POSITION_{position}_INSTALLATION_CHANGED";
    }
}
