using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class MigratedDatabaseCollection : ICollectionFixture<MigratedDatabaseFixture>
{
    public const string Name = "Migrated database";
}
