using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation
{
    public sealed class PhaseFrame
    {
        private readonly QualityTimeline _timeline;
        private readonly Dictionary<string, IReadOnlyList<double?>> _numbers = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IReadOnlyList<string?>> _texts = new(StringComparer.Ordinal);
        private readonly Dictionary<string, bool[]> _masks = new(StringComparer.Ordinal);
        private readonly Dictionary<string, DerivedChannel> _derivedChannels = new(StringComparer.Ordinal);
        private readonly Dictionary<string, AnchorResult> _anchors = new(StringComparer.Ordinal);
        private readonly List<AnchorResult> _anchorsInOrder = [];
        private readonly Dictionary<string, PhaseCondition> _conditions = new(StringComparer.Ordinal);
        private readonly Dictionary<string, bool[]> _conditionMembers = new(StringComparer.Ordinal);

        public PhaseFrame(QualityTimeline timeline, QualityResult quality)
        {
            _timeline = timeline;
            Quality = quality;

            foreach (var span in quality.Spans)
                MarkMask(span);
        }

        public QualityResult Quality { get; }

        public int Count => _timeline.Count;

        public double[] Elapsed => _timeline.Elapsed;

        public IReadOnlyList<int> GapStarts => _timeline.GapStarts;

        public IReadOnlyList<AnchorResult> Anchors => _anchorsInOrder;

        public bool StartsAfterGap(int index) => _timeline.StartsAfterGap(index);

        public bool SameBlock(int first, int second) => _timeline.Block[first] == _timeline.Block[second];

        public double RunSeconds(int start, int end) => Elapsed[end] - Elapsed[start] + 1;

        public int FirstAtOrAfter(double elapsed)
        {
            var low = 0;
            var high = Count;

            while (low < high)
            {
                var middle = (low + high) / 2;

                if (Elapsed[middle] < elapsed)
                    low = middle + 1;
                else
                    high = middle;
            }

            return low;
        }

        public IReadOnlyList<double?> NumbersOf(string channel)
        {
            if (!_numbers.TryGetValue(channel, out var values))
            {
                values = _timeline.Numbers(channel);
                _numbers[channel] = values;
            }

            return values;
        }

        public double? Number(string channel, int index) => NumbersOf(channel)[index];

        public string? Text(string channel, int index)
        {
            if (!_texts.TryGetValue(channel, out var values))
            {
                values = _timeline.Texts(channel);
                _texts[channel] = values;
            }

            return values[index];
        }

        public bool IsRecorded(string parameterKey) => _timeline.IsRecorded(parameterKey);

        public IReadOnlyList<string> RecordedKeysOf(string parameterCode)
        {
            var positionKeys = _timeline.PositionKeysOf(parameterCode);

            if (positionKeys.Count > 0)
                return positionKeys;

            return IsRecorded(parameterCode) ? [parameterCode] : [];
        }

        public bool InMask(string code, int index) => _masks.TryGetValue(code, out var mask) && mask[index];

        public bool OverlapsMask(string code, int start, int end)
        {
            for (var index = start; index <= end; index++)
            {
                if (InMask(code, index))
                    return true;
            }

            return false;
        }

        public IReadOnlyList<QualitySpan> SpansAt(int index)
            => Quality.Spans
                .Where(span => span.StartIndex <= index && index <= span.EndIndexInclusive)
                .ToList();

        public void AddDerivedChannel(DerivedChannel channel, IReadOnlyList<double?> values)
        {
            _derivedChannels[channel.Code!] = channel;
            _numbers[channel.Code!] = values;
        }

        public IReadOnlyList<DerivedValueTrace> DerivedValuesAt(int index)
            => _derivedChannels.Values
                .Select(channel => channel.Trace(this, index))
                .ToList();

        public void AddAnchor(AnchorResult anchor)
        {
            _anchors[anchor.Code] = anchor;
            _anchorsInOrder.Add(anchor);
        }

        public bool IsDerived(string channel) => _derivedChannels.ContainsKey(channel);

        public int? AnchorIndex(string code) => _anchors[code].Index;

        public void AddCondition(PhaseCondition condition, bool[] members)
        {
            _conditions[condition.Code!] = condition;
            _conditionMembers[condition.Code!] = members;
        }

        public bool InCondition(string code, int index) => _conditionMembers[code][index];

        public ConditionTrace TraceCondition(string code, int index) => _conditions[code].Trace(this, index);

        public bool[] SamplesWhere(Func<int, bool> test)
        {
            var samples = new bool[Count];

            for (var index = 0; index < Count; index++)
                samples[index] = test(index);

            return samples;
        }

        public IReadOnlyList<(int Start, int End)> RunsOf(IReadOnlyList<bool> flags) => _timeline.RunsOf(flags);

        public (int Start, int End) RunAround(int index, Func<int, bool> test)
        {
            var start = index;
            var end = index;

            while (start > 0 && !StartsAfterGap(start) && test(start - 1))
                start--;

            while (end + 1 < Count && !StartsAfterGap(end + 1) && test(end + 1))
                end++;

            return (start, end);
        }

        public static void Mark(bool[] samples, int start, int end)
        {
            for (var index = start; index <= end; index++)
                samples[index] = true;
        }

        private void MarkMask(QualitySpan span)
        {
            if (!_masks.TryGetValue(span.Code, out var mask))
            {
                mask = new bool[Count];
                _masks[span.Code] = mask;
            }

            Mark(mask, span.StartIndex, span.EndIndexInclusive);
        }
    }
}
