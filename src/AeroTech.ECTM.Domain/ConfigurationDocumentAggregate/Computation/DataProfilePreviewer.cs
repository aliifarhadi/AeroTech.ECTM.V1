using System.Globalization;
using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation
{
    public static class DataProfilePreviewer
    {
        public const string Ok = "OK";
        public const string Missing = "MISSING";
        public const string Unparseable = "UNPARSEABLE";
        public const string Text = "TEXT";

        public const int RowsShown = 10;

        public static ProfilePreview Preview(DataProfileContent profile, ValidatedCatalog catalog, IEnumerable<IReadOnlyList<string>> rows)
        {
            var columns = profile.Columns!.OrderBy(column => column.Ordinal).ToList();
            var expectedColumnCount = profile.ColumnCount!.Value;

            using var enumerator = rows.GetEnumerator();

            if (!enumerator.MoveNext())
                throw ExceptionFactory.SourceFileIsUnreadable("the file has no header row");

            var header = enumerator.Current;
            var mismatches = HeaderMismatchesOf(columns, header);
            var signatureMatches = header.Count == expectedColumnCount && mismatches.Count == 0;
            var engines = EnginesOf(columns);

            if (!signatureMatches)
                return new ProfilePreview(false, expectedColumnCount, header.Count, mismatches, 0, 0, [], [], [], engines);

            var readers = columns.Select(column => ColumnReader.Of(column, catalog)).ToList();
            var firstRows = new List<PreviewRow>();
            var lastRows = new Queue<PreviewRow>();
            var dataRowCount = 0;
            var raggedRowCount = 0;

            while (enumerator.MoveNext())
            {
                var row = enumerator.Current;
                dataRowCount++;

                if (row.Count != expectedColumnCount)
                {
                    raggedRowCount++;
                    continue;
                }

                var cells = readers.Select(reader => reader.Read(row)).ToList();
                var previewRow = new PreviewRow(dataRowCount, cells);

                if (firstRows.Count < RowsShown)
                    firstRows.Add(previewRow);

                lastRows.Enqueue(previewRow);

                if (lastRows.Count > RowsShown)
                    lastRows.Dequeue();
            }

            var parseSummary = readers
                .Where(reader => reader.ParameterKey is not null)
                .Select(reader => new ColumnParseSummary(reader.Ordinal, reader.ParameterKey!, reader.MissingCount, reader.UnparseableCount))
                .ToList();

            return new ProfilePreview(true, expectedColumnCount, header.Count, [], dataRowCount, raggedRowCount, firstRows, lastRows.ToList(), parseSummary, engines);
        }

        private static List<HeaderMismatch> HeaderMismatchesOf(IReadOnlyList<DataProfileColumn> columns, IReadOnlyList<string> header)
        {
            var mismatches = new List<HeaderMismatch>();

            foreach (var column in columns)
            {
                var ordinal = column.Ordinal!.Value;
                string? actual = null;

                if (ordinal < header.Count)
                    actual = header[ordinal];

                if (actual != column.ExactHeader)
                    mismatches.Add(new HeaderMismatch(ordinal, column.ExactHeader, actual));
            }

            for (var ordinal = columns.Count; ordinal < header.Count; ordinal++)
                mismatches.Add(new HeaderMismatch(ordinal, null, header[ordinal]));

            return mismatches;
        }

        private static List<EnginePositionChannels> EnginesOf(IReadOnlyList<DataProfileColumn> columns)
            => columns
                .Where(column => column.ParameterCode is not null && column.Position is not null)
                .GroupBy(column => column.Position!.Value)
                .OrderBy(group => group.Key)
                .Select(group => new EnginePositionChannels(
                    group.Key,
                    group.Select(column => DataProfileValidator.KeyOf(column.ParameterCode!, column.Position)).ToList()))
                .ToList();

        private sealed class ColumnReader
        {
            private readonly HashSet<string> _missingTokens;
            private readonly bool _numeric;

            private ColumnReader(int ordinal, string? parameterKey, IEnumerable<string> missingTokens, bool numeric)
            {
                Ordinal = ordinal;
                ParameterKey = parameterKey;
                _missingTokens = missingTokens.Select(token => token.Trim()).ToHashSet(StringComparer.Ordinal);
                _numeric = numeric;
            }

            public int Ordinal { get; }

            public string? ParameterKey { get; }

            public int MissingCount { get; private set; }

            public int UnparseableCount { get; private set; }

            public static ColumnReader Of(DataProfileColumn column, ValidatedCatalog catalog)
            {
                string? parameterKey = null;
                var numeric = false;

                if (column.ParameterCode is { } parameterCode)
                {
                    parameterKey = DataProfileValidator.KeyOf(parameterCode, column.Position);
                    numeric = catalog.Find(parameterCode)?.IsNumeric ?? false;
                }

                return new ColumnReader(column.Ordinal!.Value, parameterKey, column.MissingTokens ?? [], numeric);
            }

            public PreviewCell Read(IReadOnlyList<string> row)
            {
                var raw = row[Ordinal];
                var trimmed = raw.Trim();

                if (_missingTokens.Contains(trimmed))
                {
                    MissingCount++;
                    return new PreviewCell(Ordinal, ParameterKey, raw, null, Missing);
                }

                if (!_numeric)
                    return new PreviewCell(Ordinal, ParameterKey, raw, null, Text);

                if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                    return new PreviewCell(Ordinal, ParameterKey, raw, number, Ok);

                UnparseableCount++;
                return new PreviewCell(Ordinal, ParameterKey, raw, null, Unparseable);
            }
        }
    }
}
