using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

[CollectionDefinition(Name)]
public sealed class IdentityDatabaseCollection : ICollectionFixture<MigratedDatabaseFixture>
{
    public const string Name = "Identity database";
}
