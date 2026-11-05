namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public static class ClockTime
    {
        public const int SecondsPerDay = 86_400;

        public static int? SecondsOf(string? text, bool acceptOneDigitHour)
        {
            if (text is null)
                return null;

            var parts = text.Trim().Split(':');

            if (parts.Length != 3 || !parts.All(part => part.Length > 0 && part.All(char.IsAsciiDigit)))
                return null;

            var hourDigitsValid = parts[0].Length == 2 || (acceptOneDigitHour && parts[0].Length == 1);

            if (!hourDigitsValid || parts[1].Length != 2 || parts[2].Length != 2)
                return null;

            var hours = int.Parse(parts[0]);
            var minutes = int.Parse(parts[1]);
            var seconds = int.Parse(parts[2]);

            if (hours >= 24 || minutes >= 60 || seconds >= 60)
                return null;

            return (hours * 3600) + (minutes * 60) + seconds;
        }

        public static double DayModulo(double seconds) => ((seconds % SecondsPerDay) + SecondsPerDay) % SecondsPerDay;
    }
}
