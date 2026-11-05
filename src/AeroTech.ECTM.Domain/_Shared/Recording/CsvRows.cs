using System.Buffers;
using System.Text;
using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class CsvRows
    {
        private const char Quote = '"';
        private const char ByteOrderMark = '﻿';

        private static readonly Encoding StrictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public static IEnumerable<IReadOnlyList<string>> Read(byte[] content, char delimiter)
        {
            var text = Decode(content);

            return RowsOf(text, delimiter);
        }

        private static string Decode(byte[] content)
        {
            string text;

            try
            {
                text = StrictUtf8.GetString(content);
            }
            catch (DecoderFallbackException)
            {
                throw ExceptionFactory.SourceFileIsUnreadable("the file is not valid UTF-8");
            }

            if (text.Length > 0 && text[0] == ByteOrderMark)
                return text[1..];

            return text;
        }

        private static IEnumerable<IReadOnlyList<string>> RowsOf(string text, char delimiter)
        {
            var plainStops = SearchValues.Create([delimiter, '\n', '\r']);
            var cells = new List<string>();
            var cell = new StringBuilder();
            var index = 0;

            while (index < text.Length)
            {
                var character = text[index];

                if (character == Quote && cell.Length == 0)
                {
                    index = ReadQuoted(text, index + 1, cell);
                    continue;
                }

                if (character == delimiter)
                {
                    cells.Add(cell.ToString());
                    cell.Clear();
                    index++;
                    continue;
                }

                if (character == '\r')
                {
                    index++;
                    continue;
                }

                if (character == '\n')
                {
                    if (cells.Count > 0 || cell.Length > 0)
                    {
                        cells.Add(cell.ToString());
                        cell.Clear();
                        yield return cells.ToArray();
                        cells.Clear();
                    }

                    index++;
                    continue;
                }

                var stop = text.AsSpan(index).IndexOfAny(plainStops);
                var end = stop < 0 ? text.Length : index + stop;

                cell.Append(text, index, end - index);
                index = end;
            }

            if (cells.Count > 0 || cell.Length > 0)
            {
                cells.Add(cell.ToString());
                yield return cells.ToArray();
            }
        }

        private static int ReadQuoted(string text, int index, StringBuilder cell)
        {
            while (index < text.Length)
            {
                var closing = text.IndexOf(Quote, index);

                if (closing < 0)
                    break;

                cell.Append(text, index, closing - index);

                var escaped = closing + 1 < text.Length && text[closing + 1] == Quote;

                if (!escaped)
                    return closing + 1;

                cell.Append(Quote);
                index = closing + 2;
            }

            throw ExceptionFactory.SourceFileIsUnreadable("a quoted cell is not terminated");
        }
    }
}
