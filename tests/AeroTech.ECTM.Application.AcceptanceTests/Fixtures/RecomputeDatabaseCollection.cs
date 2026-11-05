using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class RecomputeDatabaseCollection : ICollectionFixture<MigratedDatabaseFixture>
{
    public const string Name = "Recompute database";
}
