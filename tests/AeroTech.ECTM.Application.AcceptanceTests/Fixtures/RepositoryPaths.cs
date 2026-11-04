namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public static class RepositoryPaths
{
    private const string SolutionFile = "AeroTech.ECTM.sln";

    private static readonly string Root = FindRoot();

    public static string ServiceHost(string fileName) => Path.Combine(Root, "src", "AeroTech.ECTM.ServiceHost", fileName);

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, SolutionFile)))
            directory = directory.Parent;

        return directory?.FullName ?? throw new InvalidOperationException($"{SolutionFile} was not found above the test output.");
    }
}
