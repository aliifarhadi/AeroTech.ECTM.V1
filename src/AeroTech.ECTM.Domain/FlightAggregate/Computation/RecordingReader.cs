using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class RecordingReader
    {
        public const int RowsShown = 10;

        public static ParsedRecording Read(DataProfileContent profile, ValidatedCatalog catalog, IEnumerable<IReadOnlyList<string>> rows)
        {
            var columns = profile.Columns!.OrderBy(column => column.Ordinal).ToList();
            var expectedColumnCount = profile.ColumnCount!.Value;
            var enginePositions = EnginePositionsOf(columns);

            using var enumerator = rows.GetEnumerator();

            if (!enumerator.MoveNext())
                throw ExceptionFactory.SourceFileIsUnreadable("the file has no header row");

            var header = enumerator.Current;
            var mismatches = DataProfilePreviewer.HeaderMismatchesOf(columns, header);
            var signatureMatches = header.Count == expectedColumnCount && mismatches.Count == 0;

            if (!signatureMatches)
                return new ParsedRecording(false, expectedColumnCount, header.Count, mismatches, 0, null, [], [], [], enginePositions, []);

            var readers = columns
                .Where(column => column.ParameterCode is not null)
                .Select(column => ChannelReader.Of(column, catalog))
                .ToList();
            var firstRows = new List<RecordingRow>();
            var lastRows = new Queue<(int SampleIndex, IReadOnlyList<string> Cells)>();
            var sampleCount = 0;
            var raggedRowCount = 0;

            while (enumerator.MoveNext())
            {
                var row = enumerator.Current;
                var sampleIndex = sampleCount;
                sampleCount++;

                if (row.Count != expectedColumnCount)
                    raggedRowCount++;

                foreach (var reader in readers)
                    reader.Accept(row);

                if (firstRows.Count < RowsShown)
                    firstRows.Add(RowOf(readers, sampleIndex, row));

                lastRows.Enqueue((sampleIndex, row));

                if (lastRows.Count > RowsShown)
                    lastRows.Dequeue();
            }

            var shownLastRows = lastRows.Select(last => RowOf(readers, last.SampleIndex, last.Cells)).ToList();
            var channels = readers.Select(reader => reader.ToChannel()).ToList();
            var timeChannel = channels.FirstOrDefault(channel => channel.ParameterCode == profile.TimeParameterCode && channel.Position is null);
            var findings = RecordingChecks.Of(timeChannel, channels, raggedRowCount, expectedColumnCount, catalog);

            return new ParsedRecording(true, expectedColumnCount, header.Count, [], sampleCount, timeChannel, channels, firstRows, shownLastRows, enginePositions, findings);
        }

        private static RecordingRow RowOf(IReadOnlyList<ChannelReader> readers, int sampleIndex, IReadOnlyList<string> row)
            => new(sampleIndex, readers.Select(reader => reader.CellOf(row)).ToList());

        private static List<int> EnginePositionsOf(IReadOnlyList<DataProfileColumn> columns)
            => columns
                .Where(column => column.ParameterCode is not null)
                .Where(column => column.Position is not null)
                .Select(column => column.Position!.Value)
                .Distinct()
                .Order()
                .ToList();

        private sealed class ChannelReader
        {
            private readonly int _ordinal;
            private readonly DataProfileColumn _column;
            private readonly string _parameterKey;
            private readonly string _valueKind;
            private readonly HashSet<string> _missingTokens;
            private readonly PlausibilityRange? _plausibility;
            private readonly HashSet<string>? _enumValues;
            private readonly List<double?>? _numbers;
            private readonly List<string?>? _texts;
            private readonly Dictionary<string, string> _literals = new(StringComparer.Ordinal);
            private readonly Dictionary<string, int> _literalsOutsideDomain = new(StringComparer.Ordinal);
            private int _missingCount;
            private int _unparseableCount;
            private int _outOfRangeCount;

            private ChannelReader(DataProfileColumn column, CatalogEntry? entry)
            {
                _ordinal = column.Ordinal!.Value;
                _column = column;
                _parameterKey = DataProfileValidator.KeyOf(column.ParameterCode!, column.Position);
                _valueKind = entry?.ValueKind ?? "TEXT";
                _missingTokens = (column.MissingTokens ?? []).Select(token => token.Trim()).ToHashSet(StringComparer.Ordinal);
                _plausibility = entry?.Plausibility;

                if (entry?.EnumValues is { } enumValues)
                    _enumValues = enumValues.Select(value => value.Trim()).ToHashSet(StringComparer.Ordinal);

                if (entry?.IsNumeric == true)
                    _numbers = new List<double?>();
                else
                    _texts = new List<string?>();
            }

            public static ChannelReader Of(DataProfileColumn column, ValidatedCatalog catalog)
                => new(column, catalog.Find(column.ParameterCode!));

            public void Accept(IReadOnlyList<string> row)
            {
                var trimmed = RawOf(row).Trim();

                if (_missingTokens.Contains(trimmed))
                {
                    _missingCount++;
                    _numbers?.Add(null);
                    _texts?.Add(null);
                    return;
                }

                if (_texts is not null)
                {
                    var literal = Interned(trimmed);
                    _texts.Add(literal);
                    CountLiteral(literal);
                    return;
                }

                if (!RecordedNumbers.TryParse(trimmed, out var number))
                {
                    _unparseableCount++;
                    _numbers!.Add(null);
                    return;
                }

                _numbers!.Add(number);
                CountRange(number);
            }

            public PreviewCell CellOf(IReadOnlyList<string> row)
            {
                var raw = RawOf(row);
                var trimmed = raw.Trim();

                if (_missingTokens.Contains(trimmed))
                    return new PreviewCell(_ordinal, _parameterKey, raw, null, DataProfilePreviewer.Missing);

                if (_texts is not null)
                    return new PreviewCell(_ordinal, _parameterKey, raw, null, DataProfilePreviewer.Text);

                if (!RecordedNumbers.TryParse(trimmed, out var number))
                    return new PreviewCell(_ordinal, _parameterKey, raw, null, DataProfilePreviewer.Unparseable);

                return new PreviewCell(_ordinal, _parameterKey, raw, number, DataProfilePreviewer.Ok);
            }

            public RecordedChannel ToChannel()
                => new(
                    _ordinal,
                    _parameterKey,
                    _column.ParameterCode!,
                    _column.Position,
                    _column.SourceUnit!,
                    _valueKind,
                    _numbers,
                    _texts,
                    _missingCount,
                    _unparseableCount,
                    _outOfRangeCount,
                    _literalsOutsideDomain);

            private string RawOf(IReadOnlyList<string> row) => _ordinal < row.Count ? row[_ordinal] : string.Empty;

            private string Interned(string literal)
            {
                if (_literals.TryGetValue(literal, out var known))
                    return known;

                _literals[literal] = literal;

                return literal;
            }

            private void CountLiteral(string literal)
            {
                if (_enumValues is null || _enumValues.Contains(literal))
                    return;

                _literalsOutsideDomain[literal] = _literalsOutsideDomain.GetValueOrDefault(literal) + 1;
            }

            private void CountRange(double number)
            {
                if (_plausibility is not { Min: { } min, Max: { } max })
                    return;

                if (number < min || number > max)
                    _outOfRangeCount++;
            }
        }
    }
}
