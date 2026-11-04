using System.Text.RegularExpressions;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Architecture;

public sealed class AntiRegressionScanTests
{
    private static readonly Regex RecorderOrdinalAccess = new(@"SourceOrdinal|\brow\[\d+\]|\.ElementAt\(\d+\)", RegexOptions.Compiled);

    private static readonly Regex AbsolutePathLiteral = new(@"""(?:[A-Za-z]:[\\/]|\\\\\\\\|/home/|/Users/)", RegexOptions.Compiled);

    [Fact]
    public void No_source_file_reads_a_recorder_ordinal()
    {
        var offenders = Offenders(RepositoryPaths.Source(), RecorderOrdinalAccess);

        Assert.Empty(offenders);
    }

    [Fact]
    public void No_test_reads_a_path_outside_the_repository()
    {
        var offenders = Offenders(RepositoryPaths.Tests(), AbsolutePathLiteral);

        Assert.Empty(offenders);
    }

    private static List<string> Offenders(string directory, Regex pattern)
    {
        var offenders = new List<string>();

        foreach (var file in RepositoryPaths.CodeFiles(directory))
        {
            var lines = File.ReadAllLines(file);

            for (var index = 0; index < lines.Length; index++)
            {
                if (pattern.IsMatch(lines[index]))
                    offenders.Add($"{Path.GetRelativePath(RepositoryPaths.Root, file)}:{index + 1}");
            }
        }

        return offenders;
    }
}
