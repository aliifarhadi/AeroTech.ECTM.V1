using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineAggregate.Computation;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.ConformanceTests.Fakes;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConformanceTests.Baselines;

internal static class TestBaselines
{
    public const long EngineId = 1001;
    public const long CatalogDocumentId = 11;
    public const long SnapshotRulesDocumentId = 15;
    public const string Celsius = "CELSIUS";
    public const string Selector = "OPERATOR_60S";

    public static readonly DateOnly EffectiveDate = new(2026, 10, 1);
    public static readonly DateTimeOffset Now = new(2026, 11, 7, 8, 0, 0, TimeSpan.Zero);

    public static BaselineParameterArgs Parameter(
        double? rawValue,
        string parameterCode = "EGT_C",
        string? unit = Celsius,
        SnapshotKind kind = SnapshotKind.CRUISE,
        string? selector = Selector,
        double? suggestedRawValue = null,
        MeasurementContext? referenceContext = null)
        => new(
            kind,
            selector,
            parameterCode,
            unit,
            rawValue,
            suggestedRawValue,
            1,
            null,
            null,
            null,
            referenceContext ?? MeasurementContext.Unknown,
            CatalogDocumentId,
            1,
            SnapshotRulesDocumentId,
            1,
            null);

    public static BaselineArgs Description(DateOnly? effectiveDate = null, string? sourceFileSha256 = null)
        => new("Post shop reference", effectiveDate ?? EffectiveDate, null, true, null, sourceFileSha256, null);

    public static Baseline Draft(params BaselineParameterArgs[] parameters)
        => Baseline.Draft(1, EngineId, Description(), parameters, new SequenceIds(), "engineer", Now);

    public static Baseline Published(BaselineParameterArgs parameter, DateOnly? effectiveDate = null, string? sourceFileSha256 = null, long engineId = EngineId)
    {
        var baseline = Baseline.Draft(1, engineId, Description(effectiveDate, sourceFileSha256), [parameter], new SequenceIds(), "engineer", Now);
        baseline.Publish(baseline.DraftRevision, "engineer", Now);

        return baseline;
    }

    public static ObservedMeasurement Observed(
        double? value,
        string parameterCode = "EGT_C",
        string? unit = Celsius,
        long engineId = EngineId,
        SnapshotKind kind = SnapshotKind.CRUISE,
        string? selector = Selector,
        DateOnly? flightDate = null,
        string? sourceFileSha256 = null,
        long snapshotRulesDocumentId = SnapshotRulesDocumentId,
        MeasurementContext? context = null,
        SnapshotContextStatus? contextStatus = null)
    {
        var source = new FlightSource(engineId, 501, flightDate ?? EffectiveDate.AddDays(5), sourceFileSha256, 601, 701, snapshotRulesDocumentId, 1, 1);

        return new ObservedMeasurement(source, kind, selector, parameterCode, unit, value, 10, 69, null, context ?? MeasurementContext.Unknown, contextStatus);
    }

    public static MeasurementContext Context(double altitudeFt, double mach, double tatC, double n1Pct, string configuration = "On|On|Off|Off|Off|On|On")
        => new(
            [new ConditionValue("PRESSURE_ALT_FT", altitudeFt), new ConditionValue("MACH", mach), new ConditionValue("TAT_C", tatC)],
            [new ConditionValue("N1_PCT", n1Pct)],
            configuration);
}
