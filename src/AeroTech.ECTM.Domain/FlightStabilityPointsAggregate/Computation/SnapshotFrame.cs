using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation
{
    public sealed class SnapshotFrame
    {
        private readonly QualityTimeline _timeline;
        private readonly Dictionary<string, IReadOnlyList<double?>> _numbers = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IReadOnlyList<string?>> _texts = new(StringComparer.Ordinal);
        private readonly bool[] _untrusted;
        private readonly bool[] _coherenceSuspect;
        private readonly bool[] _masked;
        private readonly bool[] _cruise;
        private string[]? _configurations;

        public SnapshotFrame(SnapshotRulesContent rules, QualityTimeline timeline, QualityResult quality, PhaseResult phases)
        {
            Rules = rules;
            Quality = quality;
            Phases = phases;
            _timeline = timeline;
            _untrusted = MaskOf(quality, rules.Masks!.Untrusted!);
            _coherenceSuspect = MaskOf(quality, rules.Masks.Coherence!);
            _masked = MaskOf(quality, quality.Spans.Select(span => span.Code).Distinct().ToList());
            _cruise = PhaseSamples(phases, rules.Cruise!.Phase!.Value);

            if (rules.Masks.GapNeighboursUntrusted == true)
                MarkGapNeighbours(_untrusted);

            Positions = PositionsOf(timeline, rules.Channels!.EngineN1!);
        }

        public SnapshotRulesContent Rules { get; }

        public QualityResult Quality { get; }

        public PhaseResult Phases { get; }

        public IReadOnlyList<int> Positions { get; }

        public int Count => _timeline.Count;

        public double[] Elapsed => _timeline.Elapsed;

        public IReadOnlyList<int> GapStarts => _timeline.GapStarts;

        public bool PartialStart => Quality.PartialStart == true;

        public bool SameBlock(int first, int second) => _timeline.Block[first] == _timeline.Block[second];

        public bool StartsAfterGap(int index) => _timeline.StartsAfterGap(index);

        public bool IsUntrusted(int index) => _untrusted[index];

        public bool IsCoherenceSuspect(int index) => _coherenceSuspect[index];

        public bool IsMasked(int index) => _masked[index];

        public bool IsCruise(int index) => _cruise[index];

        public int? AnchorIndex(string code) => Phases.AnchorOf(code)?.Index;

        public static string KeyOf(string parameterCode, int position) => $"{parameterCode}[{position}]";

        public IReadOnlyList<double?> Numbers(string key)
        {
            if (!_numbers.TryGetValue(key, out var values))
            {
                values = _timeline.Numbers(key);
                _numbers[key] = values;
            }

            return values;
        }

        public string? Text(string key, int index)
        {
            if (!_texts.TryGetValue(key, out var values))
            {
                values = _timeline.Texts(key);
                _texts[key] = values;
            }

            return values[index];
        }

        public string ConfigurationAt(int index)
        {
            _configurations ??= Enumerable.Range(0, Count)
                .Select(sample => string.Join("|", Rules.ConfigurationChannels!.Select(key => Text(key, sample) ?? string.Empty)))
                .ToArray();

            return _configurations[index];
        }

        public int FirstAtOrAfter(double elapsed)
        {
            var index = 0;

            while (index < Count && Elapsed[index] < elapsed)
                index++;

            return Math.Min(index, Count - 1);
        }

        public int LastAtOrBefore(double elapsed)
        {
            var index = Count - 1;

            while (index > 0 && Elapsed[index] > elapsed)
                index--;

            return index;
        }

        public IEnumerable<int> Range(int start, int end)
        {
            for (var index = start; index <= end; index++)
                yield return index;
        }

        public IReadOnlyList<(int Start, int End)> RunsOf(IReadOnlyList<bool> flags) => _timeline.RunsOf(flags);

        public ParameterSummary SummaryOf(string key, IReadOnlyList<int> indices)
            => ParameterSummary.Of(key, indices.Select(index => Numbers(key)[index]));

        private void MarkGapNeighbours(bool[] mask)
        {
            foreach (var gapStart in _timeline.GapStarts)
            {
                mask[gapStart - 1] = true;
                mask[gapStart] = true;
            }
        }

        private static bool[] MaskOf(QualityResult quality, IReadOnlyCollection<string> codes)
        {
            var mask = new bool[quality.SampleCount];

            foreach (var span in quality.Spans.Where(span => codes.Contains(span.Code)))
            {
                for (var index = span.StartIndex; index <= span.EndIndexInclusive; index++)
                    mask[index] = true;
            }

            return mask;
        }

        private static bool[] PhaseSamples(PhaseResult phases, FlightPhaseCode phase)
        {
            var samples = new bool[phases.SampleCount];

            foreach (var segment in phases.Segments.Where(segment => segment.PhaseCode == phase))
            {
                for (var index = segment.StartIndex; index <= segment.EndIndexInclusive; index++)
                    samples[index] = true;
            }

            return samples;
        }

        private static IReadOnlyList<int> PositionsOf(QualityTimeline timeline, string engineN1)
            => timeline.PositionKeysOf(engineN1)
                .Select(key => ParameterReference.Parse(key)?.Position)
                .OfType<int>()
                .Order()
                .ToList();
    }
}
