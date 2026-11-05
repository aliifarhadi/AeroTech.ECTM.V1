using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public static class StabilityPointsEvaluator
    {
        public static StabilityPointsEvaluation Evaluate(SnapshotRulesContent rules, QualityInput input, QualityResult quality, PhaseResult? phases)
        {
            var timeline = QualityTimeline.Of(input);

            if (timeline is null || phases is not { IsEvaluated: true })
                return NotEvaluable(rules, input.SampleCount);

            var frame = new SnapshotFrame(rules, timeline, quality, phases);
            var n1Peaks = new List<TakeoffN1PeakContext>();
            var egtPeaks = new List<TakeoffEgtPeakObservation>();

            foreach (var position in frame.Positions)
            {
                var search = TakeoffSearch.Find(frame, position);
                var n1Peak = TakeoffN1PeakFinder.Find(frame, position, search);

                n1Peaks.Add(n1Peak);
                egtPeaks.Add(TakeoffEgtPeakFinder.Find(frame, position, search, n1Peak));
            }

            var cruise = rules.Cruise!.Selectors!
                .Select(selector => CruiseOf(frame, selector))
                .ToList();
            var points = new StabilityPointsResult(RuleDocumentHeader.Of(rules), frame.Count, null, frame.Positions, n1Peaks, egtPeaks, cruise);
            var metrics = new PhaseMetricsResult(PhaseMetricsCalculator.Calculate(frame, cruise), ControlContextCalculator.Calculate(frame));

            return new StabilityPointsEvaluation(points, metrics);
        }

        private static CruiseSelectorResult CruiseOf(SnapshotFrame frame, CruiseSelector selector)
        {
            var engines = frame.Positions
                .Select(position => CruiseBlockFinder.Find(frame, selector, position))
                .ToList();

            return CruiseSelectorResult.Of(selector.Code!, engines, PairedComparer.Compare(frame, selector, engines));
        }

        private static StabilityPointsEvaluation NotEvaluable(SnapshotRulesContent rules, int sampleCount)
        {
            var points = new StabilityPointsResult(RuleDocumentHeader.Of(rules), sampleCount, SnapshotReason.NO_PHASE_RESULT, [], [], [], []);

            return new StabilityPointsEvaluation(points, new PhaseMetricsResult([], null));
        }
    }
}
