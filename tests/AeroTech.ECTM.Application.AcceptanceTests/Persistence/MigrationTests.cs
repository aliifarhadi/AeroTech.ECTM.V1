using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Persistence;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class MigrationTests(MigratedDatabaseFixture fixture)
{
    [Fact]
    public async Task The_test_database_has_every_migration_applied()
    {
        await using var context = fixture.NewContext(FixedActorResolver.Alice);

        var pending = await context.Database.GetPendingMigrationsAsync();

        Assert.Empty(pending);
    }

    [Fact]
    public async Task The_model_has_no_change_missing_from_the_migrations()
    {
        await using var context = fixture.NewContext(FixedActorResolver.Alice);

        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public void The_test_database_is_a_throwaway_database()
    {
        Assert.StartsWith("DotAirEctmV1_Test_", fixture.Database.Name);
    }
}
