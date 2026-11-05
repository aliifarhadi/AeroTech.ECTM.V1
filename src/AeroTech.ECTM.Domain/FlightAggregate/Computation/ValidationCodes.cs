namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class ValidationCodes
    {
        public const string TimeAxisValueMissing = "TIME_AXIS_VALUE_MISSING";
        public const string TimeAxisDuplicate = "TIME_AXIS_DUPLICATE";
        public const string TimeAxisDecreasing = "TIME_AXIS_DECREASING";
        public const string TimeAxisGap = "TIME_AXIS_GAP";
        public const string RaggedRows = "RAGGED_ROWS";
        public const string ChannelCoverage = "CHANNEL_COVERAGE";
        public const string PlausibilityOutOfRange = "PLAUSIBILITY_OUT_OF_RANGE";
        public const string EnumLiteralOutsideDomain = "ENUM_LITERAL_OUTSIDE_DOMAIN";
    }
}
