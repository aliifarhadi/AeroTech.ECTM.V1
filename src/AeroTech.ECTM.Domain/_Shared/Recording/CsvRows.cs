using System.Text;
using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class CsvRows
    {
        private const char Quote = '"';

        private static readonly Encoding StrictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public static IEnumerable<IReadOnlyList<string>> Read(byte[] content, char delimiter)
        {
            using var stream = new MemoryStream(content, writable: false);
            using var reader = new StreamReader(stream, StrictUtf8, detectEncodingFromByteOrderMarks: true);

            var cells = new List<string>();
            var cell = new StringBuilder();
            var quoted = false;
            int next;

            while ((next = ReadCharacter(reader)) >= 0)
            {
                var character = (char)next;

                if (quoted)
                {
                    if (character != Quote)
                        cell.Append(character);
                    else if (reader.Peek() == Quote)
                        cell.Append((char)reader.Read());
                    else
                        quoted = false;

                    continue;
                }

                if (character == Quote && cell.Length == 0)
                {
                    quoted = true;
                    continue;
                }

                if (character == delimiter)
                {
                    cells.Add(cell.ToString());
                    cell.Clear();
                    continue;
                }

                if (character == '\r')
                    continue;

                if (character == '\n')
                {
                    if (cells.Count > 0 || cell.Length > 0)
                    {
                        cells.Add(cell.ToString());
                        cell.Clear();
                        yield return cells.ToArray();
                        cells.Clear();
                    }

                    continue;
                }

                cell.Append(character);
            }

            if (quoted)
                throw ExceptionFactory.SourceFileIsUnreadable("a quoted cell is not terminated");

            if (cells.Count > 0 || cell.Length > 0)
            {
                cells.Add(cell.ToString());
                yield return cells.ToArray();
            }
        }

        private static int ReadCharacter(StreamReader reader)
        {
            try
            {
                return reader.Read();
            }
            catch (DecoderFallbackException)
            {
                throw ExceptionFactory.SourceFileIsUnreadable("the file is not valid UTF-8");
            }
        }
    }
}
