namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class UnitCodes
    {
        public const string Ratio = "RATIO";

        public static readonly IReadOnlySet<string> Known = new HashSet<string>(StringComparer.Ordinal)
        {
            "SECOND",
            "HHMMSS",
            "ENUM",
            "NONE",
            "FOOT",
            "FOOT_PER_MINUTE",
            "KNOT",
            "MACH",
            "CELSIUS",
            "KELVIN",
            "MILLIBAR",
            "INHG",
            "POUND",
            "PPH",
            "PERCENT",
            "PSI",
            "DEGREE",
            "RECORDER_VIBRATION_NATIVE_UNVERIFIED",
            Ratio,
            "CELSIUS_PER_PERCENT"
        };
    }
}
