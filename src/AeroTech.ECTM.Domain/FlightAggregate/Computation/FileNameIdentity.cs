using System.Globalization;
using System.Text.RegularExpressions;
using AeroTech.ECTM.Domain._Shared.Recording;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public sealed record FileNameIdentity(DateOnly? FlightDate, string? Registration, string? FlightNumber)
    {
        private static readonly Regex Pattern = new(
            @"^(?<date>\d{4}-\d{2}-\d{2})_(?<registration>[A-Z0-9]+(-[A-Z0-9]+)*)_(?<flightNumber>[A-Z0-9]+)\.(csv|xlsx)$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static FileNameIdentity Parse(string fileName)
        {
            var match = Pattern.Match(SourceFileKinds.BareName(fileName));

            if (!match.Success)
                return new FileNameIdentity(null, null, null);

            DateOnly? flightDate = null;

            if (DateOnly.TryParseExact(match.Groups["date"].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                flightDate = date;

            var registration = match.Groups["registration"].Value.ToUpperInvariant();
            var flightNumber = match.Groups["flightNumber"].Value.ToUpperInvariant();

            return new FileNameIdentity(flightDate, registration, flightNumber);
        }
    }
}
