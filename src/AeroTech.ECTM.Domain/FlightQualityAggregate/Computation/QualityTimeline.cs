using AeroTech.ECTM.Domain.FlightAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightQualityAggregate.Computation
{
    public sealed class QualityTimeline
    {
        private readonly QualityInput _input;
        private readonly HashSet<int> _gapStarts;

        private QualityTimeline(QualityInput input, double[] elapsed, IReadOnlyList<TimeGap> gaps)
        {
            _input = input;
            Elapsed = elapsed;
            _gapStarts = gaps.Select(gap => gap.AfterSampleIndex + 1).ToHashSet();
            GapStarts = _gapStarts.Order().ToList();
            Block = BlocksOf(elapsed.Length, _gapStarts);
        }

        public int Count => Elapsed.Length;

        public double[] Elapsed { get; }

        public int[] Block { get; }

        public IReadOnlyList<int> GapStarts { get; }

        public static QualityTimeline? Of(QualityInput input)
        {
            var axis = TimeAxis.Of(input.Elapsed);
            var increasing = axis.MissingCount == 0 && axis.DuplicateCount == 0 && axis.DecreasingCount == 0;

            if (input.Elapsed.Count == 0 || !increasing)
                return null;

            var elapsed = input.Elapsed.Select(value => value!.Value).ToArray();

            return new QualityTimeline(input, elapsed, axis.Gaps);
        }

        public bool StartsAfterGap(int index) => _gapStarts.Contains(index);

        public bool IsRecorded(string parameterKey)
            => _input.Numbers.ContainsKey(parameterKey) || _input.Texts.ContainsKey(parameterKey);

        public IReadOnlyList<double?> Numbers(string parameterKey)
            => _input.Numbers.TryGetValue(parameterKey, out var values) ? values : new double?[Count];

        public IReadOnlyList<string?> Texts(string parameterKey)
            => _input.Texts.TryGetValue(parameterKey, out var values) ? values : new string?[Count];

        public int OrdinalOf(string parameterKey)
            => _input.Channels.FirstOrDefault(channel => channel.ParameterKey == parameterKey)?.ProfileOrdinal ?? -1;

        public IReadOnlyList<string> PositionKeysOf(string parameterCode)
            => _input.Channels
                .Where(channel => channel.ParameterCode == parameterCode && channel.ParameterKey != parameterCode)
                .Select(channel => channel.ParameterKey)
                .Order(StringComparer.Ordinal)
                .ToList();

        public IReadOnlyList<(int Start, int End)> RunsOf(IReadOnlyList<bool> flags)
        {
            var runs = new List<(int Start, int End)>();
            var index = 0;

            while (index < flags.Count)
            {
                if (!flags[index])
                {
                    index++;
                    continue;
                }

                var end = index;

                while (end + 1 < flags.Count && flags[end + 1] && !StartsAfterGap(end + 1))
                    end++;

                runs.Add((index, end));
                index = end + 1;
            }

            return runs;
        }

        private static int[] BlocksOf(int count, IReadOnlySet<int> gapStarts)
        {
            var blocks = new int[count];

            for (var index = 1; index < count; index++)
                blocks[index] = blocks[index - 1] + (gapStarts.Contains(index) ? 1 : 0);

            return blocks;
        }
    }
}
