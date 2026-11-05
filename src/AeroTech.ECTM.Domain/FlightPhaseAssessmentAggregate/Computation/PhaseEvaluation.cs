using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed class PhaseEvaluation
    {
        public PhaseEvaluation(PhaseRuleSetContent rules, PhaseFrame frame, PhaseLabels labels)
        {
            Rules = rules;
            Frame = frame;
            Labels = labels;
            Segments = PhaseSegmenter.Segments(rules.Segments!, frame, labels);
        }

        public PhaseRuleSetContent Rules { get; }

        public PhaseFrame Frame { get; }

        public PhaseLabels Labels { get; }

        public IReadOnlyList<PhaseSegment> Segments { get; }

        public PhaseResult Result()
        {
            var (events, eventCoverage) = EventDetector.Detect(Rules.Events!, Frame, Labels);
            var transitions = Enumerable.Range(0, Segments.Count)
                .Select(TransitionAt)
                .ToList();

            return new PhaseResult(PhaseRuleSetHeader.Of(Rules), PhaseReference.Compare(Rules), Frame.Count, null, Frame.Anchors, Coverage(), Segments, transitions, events, eventCoverage);
        }

        public PhaseExplanation Explain(int index)
        {
            var rules = TraceRules(index);
            var segment = Segments.First(candidate => candidate.StartIndex <= index && index <= candidate.EndIndexInclusive);
            var masks = Frame.SpansAt(index)
                .Select(span => new MaskTrace(span.Code, span.StartIndex, span.EndIndexInclusive, span.JumpIndex, span.Cause))
                .ToList();

            return new PhaseExplanation(
                index,
                Frame.Elapsed[index],
                Labels.Labels[index],
                Labels.UnknownReasons[index],
                Labels.FiredRules[index],
                Labels.RuleLabels[index],
                Labels.ChangedBy[index],
                segment,
                rules,
                InputsOf(rules),
                Frame.DerivedValuesAt(index),
                masks,
                Frame.Anchors);
        }

        private List<LabelRuleTrace> TraceRules(int index)
        {
            var traces = new List<LabelRuleTrace>();
            var hasFired = false;

            foreach (var rule in Rules.LabelRules!)
            {
                var when = rule.When!.Trace(Frame, index);
                var fires = when.Holds && !hasFired;

                hasFired |= when.Holds;
                traces.Add(new LabelRuleTrace(rule.Id!, rule.LabelAt(Frame, index), when.Holds, fires, when));
            }

            return traces;
        }

        private PhaseTransition TransitionAt(int position)
        {
            var segment = Segments[position];
            var index = segment.StartIndex;
            var ruleId = Labels.FiredRules[index];
            var rule = Rules.LabelRules!.FirstOrDefault(candidate => candidate.Id == ruleId);
            FlightPhaseCode? previous = position == 0 ? null : Segments[position - 1].PhaseCode;

            return new PhaseTransition(
                index,
                Frame.Elapsed[index],
                previous,
                segment.PhaseCode,
                ruleId,
                Labels.RuleLabels[index],
                Labels.UnknownReasons[index],
                Labels.ChangedBy[index],
                rule?.When!.Trace(Frame, index));
        }

        private IReadOnlyList<PhaseCoverage> Coverage()
            => Labels.Labels
                .GroupBy(label => label)
                .OrderBy(group => group.Key)
                .Select(group => new PhaseCoverage(group.Key, group.Count(), Math.Round(100.0 * group.Count() / Frame.Count, 2)))
                .ToList();

        private IReadOnlyList<ChannelValue> InputsOf(IEnumerable<LabelRuleTrace> rules)
        {
            var inputs = new Dictionary<string, ChannelValue>(StringComparer.Ordinal);

            foreach (var rule in rules)
                CollectInputs(rule.When, inputs);

            return inputs.Values
                .OrderBy(input => input.Channel, StringComparer.Ordinal)
                .ToList();
        }

        private void CollectInputs(PredicateTrace? trace, Dictionary<string, ChannelValue> inputs)
        {
            if (trace is null)
                return;

            if (trace.Channel is { } channel && !Frame.IsDerived(channel))
                inputs.TryAdd(channel, new ChannelValue(channel, trace.Observed, trace.ObservedText));

            foreach (var child in trace.Of ?? [])
                CollectInputs(child, inputs);

            CollectInputs(trace.Condition?.Predicate, inputs);
            CollectInputs(trace.Condition?.Gate, inputs);
        }
    }
}
