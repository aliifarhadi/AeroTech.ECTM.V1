using System.Globalization;
using System.IO.Compression;
using System.Xml;
using AeroTech.ECTM.Domain._Shared.Resources;

namespace AeroTech.ECTM.Domain._Shared.Recording
{
    public static class XlsxRows
    {
        private const string MainNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private const string SharedStringsEntry = "xl/sharedStrings.xml";
        private const string WorksheetFolder = "xl/worksheets/";
        private const string SharedStringType = "s";
        private const string InlineStringType = "inlineStr";

        public static IReadOnlyList<IReadOnlyList<string>> Read(byte[] content, long maximumPartBytes)
        {
            try
            {
                using var stream = new MemoryStream(content, writable: false);
                using var workbook = new ZipArchive(stream, ZipArchiveMode.Read);

                var sheet = workbook.Entries
                                .Where(entry => entry.FullName.StartsWith(WorksheetFolder, StringComparison.Ordinal))
                                .Where(entry => entry.FullName.EndsWith(".xml", StringComparison.Ordinal))
                                .OrderBy(entry => entry.FullName, StringComparer.Ordinal)
                                .FirstOrDefault()
                            ?? throw ExceptionFactory.SourceFileIsUnreadable("the workbook holds no worksheet");

                var sharedStrings = workbook.GetEntry(SharedStringsEntry);

                EnsureWithinLimit(sheet, maximumPartBytes);

                if (sharedStrings is not null)
                    EnsureWithinLimit(sharedStrings, maximumPartBytes);

                return ReadSheet(sheet, ReadSharedStrings(sharedStrings));
            }
            catch (Exception exception) when (exception is InvalidDataException or XmlException or FormatException or ArgumentOutOfRangeException)
            {
                throw ExceptionFactory.SourceFileIsUnreadable("the workbook cannot be read");
            }
        }

        private static void EnsureWithinLimit(ZipArchiveEntry entry, long maximumPartBytes)
        {
            if (entry.Length > maximumPartBytes)
                throw ExceptionFactory.SourceFileIsTooLarge(entry.Length, maximumPartBytes);
        }

        private static List<string> ReadSharedStrings(ZipArchiveEntry? entry)
        {
            var strings = new List<string>();

            if (entry is null)
                return strings;

            using var xml = XmlReader.Create(entry.Open());

            while (xml.ReadToFollowing("si", MainNamespace))
            {
                using var item = xml.ReadSubtree();

                strings.Add(ReadText(item));
            }

            return strings;
        }

        private static List<IReadOnlyList<string>> ReadSheet(ZipArchiveEntry sheet, List<string> sharedStrings)
        {
            var rows = new List<IReadOnlyList<string>>();

            using var xml = XmlReader.Create(sheet.Open());

            while (xml.ReadToFollowing("row", MainNamespace))
            {
                using var row = xml.ReadSubtree();

                rows.Add(ReadRow(row, sharedStrings));
            }

            return rows;
        }

        private static List<string> ReadRow(XmlReader row, List<string> sharedStrings)
        {
            var cells = new List<string>();

            while (row.ReadToFollowing("c", MainNamespace))
            {
                var reference = row.GetAttribute("r") ?? throw new FormatException("A cell has no reference.");
                var column = ColumnIndex(reference);
                var type = row.GetAttribute("t");

                while (cells.Count < column)
                    cells.Add(string.Empty);

                using var cell = row.ReadSubtree();

                cells.Add(ReadCell(cell, type, sharedStrings));
            }

            return cells;
        }

        private static string ReadCell(XmlReader cell, string? type, List<string> sharedStrings)
        {
            if (type == InlineStringType)
                return ReadText(cell);

            if (!cell.ReadToFollowing("v", MainNamespace))
                return string.Empty;

            var value = cell.ReadElementContentAsString();

            if (type != SharedStringType)
                return value;

            var index = int.Parse(value, CultureInfo.InvariantCulture);

            return sharedStrings[index];
        }

        private static string ReadText(XmlReader reader)
        {
            var text = string.Empty;

            while (reader.ReadToFollowing("t", MainNamespace))
                text += reader.ReadElementContentAsString();

            return text;
        }

        private static int ColumnIndex(string reference)
        {
            var index = 0;

            foreach (var character in reference.TakeWhile(char.IsLetter))
                index = (index * 26) + (char.ToUpperInvariant(character) - 'A' + 1);

            return index - 1;
        }
    }
}
