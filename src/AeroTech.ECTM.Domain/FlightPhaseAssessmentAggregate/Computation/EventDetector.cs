using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public static class EventDetector
    {
        public static (IReadOnlyList<FlightEventSpan> Spans, IReadOnlyList<EventChannelCoverage> Coverage) Detect(
            IReadOnlyList<EventDefinition> definitions,
            PhaseFrame frame,
            PhaseLabels labels)
        {
            var spans = new List<FlightEventSpan>();
            var coverage = new List<EventChannelCoverage>();

            foreach (var definition in definitions)
            {
                var keys = frame.RecordedKeysOf(definition.Parameter!);

                if (keys.Count == 0)
                {
                    coverage.Add(new EventChannelCoverage(definition.Code!, definition.Parameter!, null, EventEvaluability.MISSING, 0, 0, 0, definition.Note));
                    continue;
                }

                foreach (var key in keys)
                    coverage.Add(DetectOnChannel(definition, key, frame, labels, spans));
            }

            return (spans, coverage);
        }

        private static EventChannelCoverage DetectOnChannel(EventDefinition definition, string key, PhaseFrame frame, PhaseLabels labels, List<FlightEventSpan> spans)
        {
            var position = ParameterReference.Parse(key)?.Position;
            var valued = frame.SamplesWhere(index => frame.Text(key, index) is not null).Count(isValued => isValued);

            if (valued == 0)
                return new EventChannelCoverage(definition.Code!, key, position, EventEvaluability.MISSING, 0, 0, 0, definition.Note);

            if (definition.Enabled != true)
                return new EventChannelCoverage(definition.Code!, key, position, EventEvaluability.UNVERIFIED_ENUM, valued, 0, 0, definition.Note);

            var active = frame.SamplesWhere(index => frame.Text(key, index) == definition.ActiveValue);
            var runs = frame.RunsOf(active);

            foreach (var (start, end) in runs)
                spans.Add(SpanOf(definition, key, position, start, end, frame, labels));

            return new EventChannelCoverage(definition.Code!, key, position, EventEvaluability.EVALUATED, valued, active.Count(isActive => isActive), runs.Count, definition.Note);
        }

        private static FlightEventSpan SpanOf(EventDefinition definition, string key, int? position, int start, int end, PhaseFrame frame, PhaseLabels labels)
        {
            var values = new List<EventValue>();

            foreach (var contextKey in ContextKeysOf(definition.ValuesAtStartAndEnd!, position, frame))
                values.Add(new EventValue(contextKey, frame.Number(contextKey, start), frame.Number(contextKey, end), null));

            foreach (var contextKey in ContextKeysOf(definition.PeakDuring!, position, frame))
                values.Add(new EventValue(contextKey, null, null, PeakOf(contextKey, start, end, frame)));

            return new FlightEventSpan(
                definition.Code!,
                key,
                position,
                start,
                end,
                frame.Elapsed[start],
                frame.Elapsed[end],
                frame.RunSeconds(start, end),
                labels.Labels[start],
                labels.Labels[end],
                values);
        }

        private static IEnumerable<string> ContextKeysOf(IReadOnlyList<string> codes, int? position, PhaseFrame frame)
        {
            foreach (var code in codes)
            {
                foreach (var key in frame.RecordedKeysOf(code))
                {
                    var keyPosition = ParameterReference.Parse(key)?.Position;

                    if (position is null || keyPosition is null || keyPosition == position)
                        yield return key;
                }
            }
        }

        private static double? PeakOf(string key, int start, int end, PhaseFrame frame)
        {
            double? peak = null;

            for (var index = start; index <= end; index++)
            {
                if (frame.Number(key, index) is { } value && (peak is null || value > peak))
                    peak = value;
            }

            return peak;
        }
    }
}
