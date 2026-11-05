using System.Globalization;

namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class RecordedNumbers
    {
        public static bool TryParse(string trimmed, out double number)
        {
            var parsed = double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out number);

            return parsed && double.IsFinite(number);
        }

        public static bool TryParseWhole(string trimmed, out int number)
        {
            number = 0;

            if (!TryParse(trimmed, out var value))
                return false;

            if (value != Math.Floor(value) || value < int.MinValue || value > int.MaxValue)
                return false;

            number = (int)value;

            return true;
        }
    }
}
