using AeroTech.Messages.Ectm.Enums;



namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation

{

    public sealed record SnapshotRulesContent(

        string? RulesCode,

        string? Title,

        string? Description,

        CalibrationStatus? CalibrationStatus,

        EngineeringSignOff? EngineeringSignOff,

        string? Source,

        SnapshotChannels? Channels,

        IReadOnlyList<string>? EngineParameters,
        IReadOnlyList<string>? ConditionParameters,

        IReadOnlyList<string>? ConfigurationChannels,

        SnapshotMasks? Masks,

        TakeoffRules? Takeoff,

        CruiseRules? Cruise,

        PairedRules? Paired,

        PhaseMetricRules? PhaseMetrics,
        ControlContextRules? ControlContext,

        IReadOnlyList<RuleDecision>? Decisions);

}

