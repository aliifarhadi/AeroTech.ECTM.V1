namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public static class QualitySpanCodes
    {
        public const string AltitudeRateUntrusted = "ALTITUDE_RATE_UNTRUSTED";
        public const string AirGroundLagsRadioHeight = "AIRGROUND_LAGS_RADIO_HEIGHT";
        public const string EngineChannelsLagKinematics = "ENGINE_CHANNELS_LAG_KINEMATICS";
        public const string SensorStuck = "SENSOR_STUCK";

        public const string DiscontinuityConfirmed = "ALTITUDE_DISCONTINUITY_CONFIRMED";
        public const string DiscontinuitySingleChannel = "ALTITUDE_DISCONTINUITY_SUSPECT_SINGLE_CHANNEL";

        public static readonly IReadOnlySet<string> DiscontinuityCauses = new HashSet<string>(StringComparer.Ordinal)
        {
            DiscontinuityConfirmed,
            DiscontinuitySingleChannel
        };

        public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
        {
            AltitudeRateUntrusted,
            AirGroundLagsRadioHeight,
            EngineChannelsLagKinematics,
            SensorStuck
        };
    }
}
