using AeroTech.ECTM.Persistence;
using PersistenceRegistration = AeroTech.ECTM.Persistence.DependencyInjection;
using AeroTech.Framework.Core.Domain.Events;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class TestDatabase
{
    public const string ConnectionStringVariable = "ECTM_TEST_SQLSERVER";

    private const string DevelopmentSettings = "appsettings.Development.json";
    private const string NamePrefix = "DotAirEctmV1_Test_";

    public TestDatabase()
    {
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..8];
        var builder = new SqlConnectionStringBuilder(ServerConnectionString())
        {
            InitialCatalog = $"{NamePrefix}{stamp}_{random}"
        };

        Name = builder.InitialCatalog;
        ConnectionString = builder.ConnectionString;
    }

    public string Name { get; }

    public string ConnectionString { get; }

    public EctmDbContext NewContext(IActorResolver actorResolver, IClock clock, IIdGenerator idGenerator)
        => new(
            new DbContextOptionsBuilder<EctmDbContext>()
                .UseSqlServer(ConnectionString, sql => sql.MigrationsHistoryTable(EctmDbContext.MigrationsHistoryTable, EctmDbContext.MigrationsHistorySchema))
                .Options,
            actorResolver,
            clock,
            new IgnoringDomainEventDispatcher(),
            idGenerator);

    public async Task CreateAsync(IClock clock, IIdGenerator idGenerator)
    {
        await using var context = NewContext(new AnonymousActorResolver(), clock, idGenerator);
        await context.Database.MigrateAsync();
    }

    public async Task DropAsync(IClock clock, IIdGenerator idGenerator)
    {
        if (!Name.StartsWith(NamePrefix, StringComparison.Ordinal))
            throw new InvalidOperationException($"Refusing to drop {Name}: it is not a test database.");

        await using var context = NewContext(new AnonymousActorResolver(), clock, idGenerator);
        await context.Database.EnsureDeletedAsync();
    }

    private static string ServerConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(ConnectionStringVariable);

        if (!string.IsNullOrWhiteSpace(fromEnvironment))
            return fromEnvironment;

        var settingsPath = RepositoryPaths.ServiceHost(DevelopmentSettings);

        if (!File.Exists(settingsPath))
            throw new InvalidOperationException($"Set {ConnectionStringVariable} to a SQL Server connection string, or provide {DevelopmentSettings}.");

        return new ConfigurationBuilder()
                   .AddJsonFile(settingsPath)
                   .Build()
                   .GetConnectionString(PersistenceRegistration.ConnectionStringName)
               ?? throw new InvalidOperationException($"ConnectionStrings:{PersistenceRegistration.ConnectionStringName} is not configured.");
    }

    private sealed class AnonymousActorResolver : IActorResolver
    {
        public Actor Resolve() => Actor.Anonymous;
    }

    private sealed class IgnoringDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
