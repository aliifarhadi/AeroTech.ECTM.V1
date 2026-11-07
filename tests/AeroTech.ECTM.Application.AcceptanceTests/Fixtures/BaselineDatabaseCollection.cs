using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class BaselineDatabaseCollection : ICollectionFixture<MigratedDatabaseFixture>
{
    public const string Name = "Baseline database";
}
