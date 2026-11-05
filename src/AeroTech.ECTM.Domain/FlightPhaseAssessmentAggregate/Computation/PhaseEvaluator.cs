using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class PhaseEvaluator
    {
        public const string TimeAxisNotIncreasing = QualityEvaluator.TimeAxisNotIncreasing;

        public static PhaseResult Evaluate(PhaseRuleSetContent rules, QualityInput input, QualityResult quality)
        {
            if (Run(rules, input, quality) is not { } evaluation)
                return new PhaseResult(PhaseRuleSetHeader.Of(rules), PhaseReference.Compare(rules), input.SampleCount, TimeAxisNotIncreasing, [], [], [], [], [], []);

            return evaluation.Result();
        }

        public static PhaseExplanation? Explain(PhaseRuleSetContent rules, QualityInput input, QualityResult quality, int sampleIndex)
            => Run(rules, input, quality)?.Explain(sampleIndex);

        public static PhaseEvaluation? Run(PhaseRuleSetContent rules, QualityInput input, QualityResult quality)
        {
            var timeline = QualityTimeline.Of(input);

            if (timeline is null)
                return null;

            var frame = new PhaseFrame(timeline, quality);

            foreach (var channel in rules.DerivedChannels!)
                frame.AddDerivedChannel(channel, channel.Compute(frame));

            foreach (var anchor in rules.Anchors!)
                frame.AddAnchor(anchor.Find(frame));

            foreach (var condition in rules.Conditions!)
                frame.AddCondition(condition, condition.Members(frame));

            var labels = LabelSamples(rules, frame);

            foreach (var step in rules.PostProcessing!)
                step.Apply(frame, labels);

            return new PhaseEvaluation(rules, frame, labels);
        }

        private static PhaseLabels LabelSamples(PhaseRuleSetContent rules, PhaseFrame frame)
        {
            var labels = new PhaseLabels(frame.Count);
            var fallback = rules.Fallback!;

            for (var index = 0; index < frame.Count; index++)
            {
                var rule = rules.LabelRules!.FirstOrDefault(candidate => candidate.Matches(frame, index));

                if (rule is null)
                    labels.Assign(index, fallback.Label!.Value, fallback.UnknownReason, null);
                else
                    labels.Assign(index, rule.LabelAt(frame, index), rule.UnknownReason, rule.Id);
            }

            return labels;
        }
    }
}
