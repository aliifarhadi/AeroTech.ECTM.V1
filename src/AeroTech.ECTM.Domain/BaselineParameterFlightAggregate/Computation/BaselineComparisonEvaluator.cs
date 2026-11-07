using AeroTech.ECTM.Domain.BaselineAggregate;
using AeroTech.ECTM.Domain.BaselineAggregate.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation
{
    public static class BaselineComparisonEvaluator
    {
        public const int CalculationVersion = 1;

        public const string UnverifiedUnitSuffix = "_UNVERIFIED";

        public static BaselineComparison Evaluate(Baseline baseline, BaselineParameter parameter, ObservedMeasurement observed, ComparisonTolerances tolerances)
        {
            var reference = parameter.RawValue
                            ?? throw new InvalidOperationException($"Baseline parameter {parameter.Id} has no value to compare with.");
            var referenceContext = parameter.ReadReferenceContext();
            var detail = ComparisonDetail.Of(referenceContext, observed.Context, tolerances);

            if (IdentityMismatchOf(baseline, parameter, observed) is { } mismatch)
                return BaselineComparison.NotComparable(mismatch, observed.Value, detail);

            if (observed.Value is not { } rawObserved || !double.IsFinite(rawObserved))
                return BaselineComparison.NotComparable(ComparisonReason.MISSING_OBSERVATION, null, detail);

            var rawDelta = rawObserved - reference;
            var reasons = WarningsOf(baseline, parameter, observed, referenceContext.IsUnknown, detail);

            return new BaselineComparison(StatusOf(parameter.Unit), rawObserved, rawDelta, reasons, detail);
        }

        private static ComparisonReason? IdentityMismatchOf(Baseline baseline, BaselineParameter parameter, ObservedMeasurement observed)
        {
            if (observed.Source.EngineId != baseline.EngineId)
                return ComparisonReason.DIFFERENT_ESN;

            if (observed.SnapshotKind != parameter.SnapshotKind)
                return ComparisonReason.SNAPSHOT_KIND_MISMATCH;

            if (observed.Selector != parameter.Selector)
                return ComparisonReason.SELECTOR_MISMATCH;

            if (observed.ParameterCode != parameter.ParameterCode)
                return ComparisonReason.PARAMETER_MISMATCH;

            if (observed.Unit != parameter.Unit)
                return ComparisonReason.UNIT_MISMATCH;

            return null;
        }

        private static List<ComparisonReason> WarningsOf(Baseline baseline, BaselineParameter parameter, ObservedMeasurement observed, bool referenceContextUnknown, ComparisonDetail detail)
        {
            var reasons = new List<ComparisonReason> { ComparisonReason.ENVIRONMENT_NOT_REMOVED };

            if (referenceContextUnknown || observed.Context.IsUnknown)
                reasons.Add(ComparisonReason.CONTEXT_UNKNOWN);

            if (observed.ContextStatus == SnapshotContextStatus.CONTEXT_UNVERIFIED_CHANNEL_COHERENCE)
                reasons.Add(ComparisonReason.CONTEXT_UNVERIFIED_CHANNEL_COHERENCE);

            if (detail.HasConditionOutsideTolerance)
                reasons.Add(ComparisonReason.CONDITION_OUT_OF_TOLERANCE);

            if (detail.ConfigurationDiffers)
                reasons.Add(ComparisonReason.CONFIGURATION_DIFFERS);

            if (observed.Source.SnapshotRulesDocumentId != parameter.SnapshotRulesDocumentId)
                reasons.Add(ComparisonReason.SNAPSHOT_RULESET_CHANGED);

            if (IsReferenceSource(baseline, observed))
                reasons.Add(ComparisonReason.REFERENCE_SOURCE_POINT);

            if (ChronologyOf(baseline.DeclaredEffectiveDate, observed.Source.FlightDate) is { } chronology)
                reasons.Add(chronology);

            reasons.Add(ComparisonReason.NO_APPROVED_CORRECTION_MODEL);

            return reasons;
        }

        private static bool IsReferenceSource(Baseline baseline, ObservedMeasurement observed)
            => baseline.SourceFileSha256 is not null
               && baseline.SourceFileSha256 == observed.Source.SourceFileSha256;

        private static ComparisonReason? ChronologyOf(DateOnly? declaredEffectiveDate, DateOnly? flightDate)
        {
            if (declaredEffectiveDate is not { } effectiveDate || flightDate is not { } date)
                return ComparisonReason.TEMPORAL_ORDER_UNVERIFIED;

            if (date < effectiveDate)
                return ComparisonReason.RETROSPECTIVE;

            if (date == effectiveDate)
                return ComparisonReason.TEMPORAL_ORDER_UNVERIFIED;

            return null;
        }

        private static ComparisonStatus StatusOf(string? unit)
        {
            if (unit is not null && unit.EndsWith(UnverifiedUnitSuffix, StringComparison.Ordinal))
                return ComparisonStatus.DESCRIPTIVE_RAW_UNIT_UNVERIFIED;

            return ComparisonStatus.DESCRIPTIVE_RAW;
        }
    }
}
