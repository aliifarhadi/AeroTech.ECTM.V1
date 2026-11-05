namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public sealed record QualityRulesContent(
        string? RulesCode,
        string? CalibrationStatus,
        string? Source,
        QualityChannels? Channels,
        AirGroundLiterals? AirGroundLiterals,
        AltitudeDiscontinuityRules? AltitudeDiscontinuity,
        AirGroundLagRules? AirGroundLag,
        EngineLagRules? EngineLag,
        PartialRules? Partial,
        ClockRules? Clock,
        CoverageRules? Coverage,
        PlausibilityRules? Plausibility,
        StuckSensorRules? StuckSensor,
        GradeRules? Grade,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? Affects);
}
