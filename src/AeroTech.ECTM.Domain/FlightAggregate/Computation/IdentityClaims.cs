using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class IdentityClaims
    {
        public const string FlightNumberCode = "FLIGHT_NUMBER";
        public const string DayCode = "DAY";
        public const string MonthCode = "MONTH";

        public static IdentityClaim From(string fileName, string? defaultRegistration, int? defaultYear, ParsedRecording recording)
        {
            var fromName = FileNameIdentity.Parse(fileName);
            var reasons = new List<string>();

            var (registration, registrationSource) = RegistrationOf(fromName, defaultRegistration, reasons);
            var (flightNumber, flightNumberSource) = FlightNumberOf(fromName, recording.DistinctTextsOf(FlightNumberCode), reasons);
            var day = WholeValueOf(recording.DistinctTextsOf(DayCode), IdentityReasons.DayNotConstant, reasons);
            var month = WholeValueOf(recording.DistinctTextsOf(MonthCode), IdentityReasons.MonthNotConstant, reasons);
            var (flightDate, flightDateSource) = FlightDateOf(fromName, defaultYear, day, month, reasons);

            return new IdentityClaim(registration, registrationSource, flightDate, flightDateSource, flightNumber, flightNumberSource, reasons);
        }

        public static bool SameFlightNumber(string left, string right)
        {
            var leftNormalized = NormalizedFlightNumber(left);
            var rightNormalized = NormalizedFlightNumber(right);

            return string.Equals(leftNormalized, rightNormalized, StringComparison.OrdinalIgnoreCase);
        }

        private static (string?, IdentitySource?) RegistrationOf(FileNameIdentity fromName, string? defaultRegistration, List<string> reasons)
        {
            if (fromName.Registration is { } registration)
                return (registration, IdentitySource.FILE_NAME);

            if (!string.IsNullOrWhiteSpace(defaultRegistration))
                return (defaultRegistration.Trim().ToUpperInvariant(), IdentitySource.USER_DEFAULT);

            reasons.Add(IdentityReasons.RegistrationMissing);

            return (null, null);
        }

        private static (string?, IdentitySource?) FlightNumberOf(FileNameIdentity fromName, IReadOnlyList<string> columnValues, List<string> reasons)
        {
            string? columnValue = null;

            if (columnValues.Count == 1)
                columnValue = columnValues[0];

            if (columnValues.Count > 1)
                reasons.Add(IdentityReasons.FlightNumberNotConstant);

            if (fromName.FlightNumber is { } nameValue)
            {
                if (columnValue is not null && !SameFlightNumber(nameValue, columnValue))
                    reasons.Add(IdentityReasons.FlightNumberMismatch);

                return (nameValue, IdentitySource.FILE_NAME);
            }

            if (columnValue is not null)
                return (columnValue.ToUpperInvariant(), IdentitySource.COLUMNS);

            reasons.Add(IdentityReasons.FlightNumberMissing);

            return (null, null);
        }

        private static (DateOnly?, IdentitySource?) FlightDateOf(FileNameIdentity fromName, int? defaultYear, int? day, int? month, List<string> reasons)
        {
            if (fromName.FlightDate is { } nameDate)
            {
                if (day is { } columnDay && columnDay != nameDate.Day)
                    reasons.Add(IdentityReasons.DayMismatch);

                if (month is { } columnMonth && columnMonth != nameDate.Month)
                    reasons.Add(IdentityReasons.MonthMismatch);

                return (nameDate, IdentitySource.FILE_NAME);
            }

            if (ComposedDate(defaultYear, month, day) is { } composed)
                return (composed, IdentitySource.COLUMNS);

            reasons.Add(IdentityReasons.FlightDateMissing);

            return (null, null);
        }

        private static DateOnly? ComposedDate(int? year, int? month, int? day)
        {
            if (year is not { } y || month is not { } m || day is not { } d)
                return null;

            if (m < 1 || m > 12 || y < 1 || y > 9999)
                return null;

            if (d < 1 || d > DateTime.DaysInMonth(y, m))
                return null;

            return new DateOnly(y, m, d);
        }

        private static int? WholeValueOf(IReadOnlyList<string> columnValues, string notConstantReason, List<string> reasons)
        {
            if (columnValues.Count > 1)
                reasons.Add(notConstantReason);

            if (columnValues.Count != 1)
                return null;

            if (!RecordedNumbers.TryParseWhole(columnValues[0], out var value))
                return null;

            return value;
        }

        private static string NormalizedFlightNumber(string value)
        {
            var trimmed = value.Trim();
            var withoutLeadingZeros = trimmed.TrimStart('0');

            if (withoutLeadingZeros.Length == 0 && trimmed.Length > 0)
                return "0";

            return withoutLeadingZeros;
        }
    }
}
