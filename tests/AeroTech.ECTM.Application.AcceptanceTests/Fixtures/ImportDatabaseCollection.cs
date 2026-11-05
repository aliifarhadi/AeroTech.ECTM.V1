using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class ImportDatabaseCollection : ICollectionFixture<MigratedDatabaseFixture>
{
    public const string Name = "Import database";
}
