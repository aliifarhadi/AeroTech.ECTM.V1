using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public static class QualityGrader
    {
        public const string BlockingValidation = "BLOCKING_VALIDATION";
        public const string CoverageBelowBlocking = "COVERAGE_BELOW_BLOCKING";
        public const string CoverageBelowWarning = "COVERAGE_BELOW_WARNING";
        public const string PartialStartAndEnd = "PARTIAL_START_AND_END";
        public const string Mask = "MASK";
        public const string ElapsedGap = "ELAPSED_GAP";
        public const string CoverageBelowGradeA = "COVERAGE_BELOW_GRADE_A";

        public static GradeDecision Grade(
            QualityRulesContent rules,
            bool hasBlockingValidation,
            IReadOnlyList<ChannelCoverage> coverage,
            IReadOnlyList<ValidationFinding> findings,
            IReadOnlyList<QualitySpan> spans,
            IReadOnlyList<int> gapStarts,
            bool? partialStart,
            bool? partialEnd)
        {
            var coverageRules = rules.Coverage!;
            var notUsable = new List<string>();

            if (hasBlockingValidation || findings.Any(finding => finding.Code != QualityEvaluator.RequiredChannelCoverage && finding.Severity == ValidationSeverity.BLOCKING))
                notUsable.Add(BlockingValidation);

            notUsable.AddRange(CoverageReasons(coverage, coverageRules.BlockingBelowPct!.Value, CoverageBelowBlocking));

            if (notUsable.Count > 0)
                return new GradeDecision(QualityGrade.NOT_USABLE, notUsable);

            var gradeC = CoverageReasons(coverage, coverageRules.WarningBelowPct!.Value, CoverageBelowWarning);

            if (partialStart == true && partialEnd == true)
                gradeC.Add(PartialStartAndEnd);

            if (gradeC.Count > 0)
                return new GradeDecision(QualityGrade.C, gradeC);

            var allowedForA = rules.Grade!.MasksAllowedForA ?? [];
            var gradeB = spans
                .Where(span => !IsAllowedForA(span, allowedForA))
                .Select(span => span.Code)
                .Distinct(StringComparer.Ordinal)
                .Select(code => $"{Mask}:{code}")
                .ToList();

            if (gapStarts.Count > 0)
                gradeB.Add(ElapsedGap);

            gradeB.AddRange(CoverageReasons(coverage, rules.Grade.AMinCoveragePct!.Value, CoverageBelowGradeA));

            if (gradeB.Count > 0)
                return new GradeDecision(QualityGrade.B, gradeB);

            return new GradeDecision(QualityGrade.A, []);
        }

        private static bool IsAllowedForA(QualitySpan span, IReadOnlyList<AllowedMask> allowedForA)
            => allowedForA.Any(mask => mask.Code == span.Code && (mask.Cause is null || mask.Cause == span.Cause));

        private static List<string> CoverageReasons(IReadOnlyList<ChannelCoverage> coverage, double belowPercent, string reason)
            => coverage
                .Where(channel => channel.Percent < belowPercent)
                .Select(channel => $"{reason}:{channel.ParameterKey}")
                .ToList();
    }
}
