using System.Globalization;
using System.IO.Compression;
using System.Security;
using System.Text;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public static class CsvWorkbook
{
    private const string MainNamespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    public static byte[] FromCsv(byte[] csv)
    {
        var lines = Encoding.UTF8.GetString(csv).Split('\n').Select(line => line.TrimEnd('\r')).Where(line => line.Length > 0);
        var sheet = new StringBuilder($"<worksheet xmlns=\"{MainNamespace}\"><sheetData>");
        var rowNumber = 0;

        foreach (var line in lines)
        {
            rowNumber++;
            sheet.Append($"<row r=\"{rowNumber}\">");

            var cells = line.Split(',');

            for (var column = 0; column < cells.Length; column++)
                sheet.Append(CellOf($"{ColumnName(column)}{rowNumber}", cells[column]));

            sheet.Append("</row>");
        }

        sheet.Append("</sheetData></worksheet>");

        using var buffer = new MemoryStream();

        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry("xl/worksheets/sheet1.xml");

            using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
            writer.Write(sheet.ToString());
        }

        return buffer.ToArray();
    }

    private static string CellOf(string reference, string text)
    {
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
            return $"<c r=\"{reference}\"><v>{text}</v></c>";

        return $"<c r=\"{reference}\" t=\"inlineStr\"><is><t xml:space=\"preserve\">{SecurityElement.Escape(text)}</t></is></c>";
    }

    private static string ColumnName(int index)
    {
        var name = string.Empty;

        for (var number = index + 1; number > 0; number = (number - 1) / 26)
            name = (char)('A' + (number - 1) % 26) + name;

        return name;
    }
}
