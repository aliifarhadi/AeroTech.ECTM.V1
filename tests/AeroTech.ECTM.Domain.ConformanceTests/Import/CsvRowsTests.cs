using System.Text;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.Framework.Core.Domain.Exceptions;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class CsvRowsTests
{
    [Fact]
    public void Cells_keep_their_exact_text_and_blank_lines_are_skipped()
    {
        var rows = Read("﻿ Elapsed Time (seconds), AIR GROUND ()\r\n114960, GND\r\n\r\n114961,\r\n");

        Assert.Equal(3, rows.Count);
        Assert.Equal([" Elapsed Time (seconds)", " AIR GROUND ()"], rows[0]);
        Assert.Equal(["114960", " GND"], rows[1]);
        Assert.Equal(["114961", ""], rows[2]);
    }

    [Fact]
    public void Quoted_cells_may_hold_delimiters_line_breaks_and_escaped_quotes()
    {
        var rows = Read("\"a,b\",\"say \"\"hi\"\"\",\"line\r\nbreak\"\nlast");

        Assert.Equal(["a,b", "say \"hi\"", "line\r\nbreak"], rows[0]);
        Assert.Equal(["last"], rows[1]);
    }

    [Fact]
    public void A_quote_inside_an_unquoted_cell_is_text()
    {
        Assert.Equal(["5\" pipe", "x"], Read("5\" pipe,x")[0]);
    }

    [Fact]
    public void An_unterminated_quote_or_invalid_utf8_is_unreadable()
    {
        var unterminated = Assert.Throws<BusinessException>(() => CsvRows.Read(Encoding.UTF8.GetBytes("\"open,x"), ',').ToList());
        var invalid = Assert.Throws<BusinessException>(() => CsvRows.Read([0x41, 0xC3, 0x28], ',').ToList());

        Assert.Equal(80112, unterminated.Code);
        Assert.Equal(80112, invalid.Code);
    }

    private static List<IReadOnlyList<string>> Read(string text) => CsvRows.Read(Encoding.UTF8.GetBytes(text), ',').ToList();
}
