namespace AeroTech.ECTM.Domain.ConformanceTests;

public static class RepositoryPaths
{
    private const string SolutionFile = "AeroTech.ECTM.sln";

    public static readonly string Root = FindRoot();

    public static string Fixtures(params string[] segments) => Path.Combine([Root, "tests", "fixtures", .. segments]);

    public static string Source() => Path.Combine(Root, "src");

    public static string Tests() => Path.Combine(Root, "tests");

    public static IReadOnlyList<string> CodeFiles(string directory)
        => Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories)
            .Where(path => !IsBuildOutput(path))
            .Order(StringComparer.Ordinal)
            .ToList();

    private static bool IsBuildOutput(string path)
    {
        var segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        return segments.Contains("bin") || segments.Contains("obj");
    }

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, SolutionFile)))
            directory = directory.Parent;

        return directory?.FullName ?? throw new InvalidOperationException($"{SolutionFile} was not found above the test output.");
    }
}
