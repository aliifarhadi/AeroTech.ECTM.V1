using System.IO.Compression;
using System.Text;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.Framework.Core.Domain.Exceptions;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class XlsxRowsTests
{
    private const string Sheet = """
        <worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>
        <row r="1"><c r="A1" t="s"><v>0</v></c><c r="B1" t="inlineStr"><is><t> AIR GROUND ()</t></is></c></row>
        <row r="2"><c r="A2"><v>114960</v></c><c r="C2" t="s"><v>1</v></c></row>
        </sheetData></worksheet>
        """;

    private const string SharedStrings = """
        <sst xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><si><t> Elapsed Time (seconds)</t></si><si><t> GND</t></si></sst>
        """;

    [Fact]
    public void Shared_and_inline_strings_are_read_and_skipped_cells_stay_empty()
    {
        var rows = XlsxRows.Read(Workbook(), 1_000_000);

        Assert.Equal([" Elapsed Time (seconds)", " AIR GROUND ()"], rows[0]);
        Assert.Equal(["114960", "", " GND"], rows[1]);
    }

    [Fact]
    public void A_part_larger_than_the_limit_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(() => XlsxRows.Read(Workbook(), 10));

        Assert.Equal(80113, exception.Code);
    }

    [Fact]
    public void Bytes_that_are_not_a_workbook_are_unreadable()
    {
        var exception = Assert.Throws<BusinessException>(() => XlsxRows.Read("not a workbook"u8.ToArray(), 1_000_000));

        Assert.Equal(80112, exception.Code);
    }

    private static byte[] Workbook()
    {
        using var buffer = new MemoryStream();

        using (var archive = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            Write(archive, "xl/worksheets/sheet1.xml", Sheet);
            Write(archive, "xl/sharedStrings.xml", SharedStrings);
        }

        return buffer.ToArray();
    }

    private static void Write(ZipArchive archive, string name, string xml)
    {
        using var entry = archive.CreateEntry(name).Open();
        entry.Write(Encoding.UTF8.GetBytes(xml));
    }
}
