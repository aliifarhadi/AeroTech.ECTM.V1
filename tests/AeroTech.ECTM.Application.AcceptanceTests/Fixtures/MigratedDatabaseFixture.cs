using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Persistence;
using PersistenceRegistration = AeroTech.ECTM.Persistence.DependencyInjection;
using AeroTech.Framework.Core.ServiceContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class MigratedDatabaseFixture : IAsyncLifetime
{
    private RestApiHost _host = null!;

    public FixedClock Clock { get; } = new();

    public SequentialIdGenerator Ids { get; } = new();

    public InMemoryDistributedLock Locks { get; } = new();

    public TestDatabase Database { get; } = new();

    public HttpClient Client => _host.Client;

    public IServiceProvider Services => _host.Services;

    public EctmDbContext NewContext(IActorResolver actorResolver) => Database.NewContext(actorResolver, Clock, Ids);

    public async Task InitializeAsync()
    {
        await Database.CreateAsync(Clock, Ids);

        _host = await RestApiHost.StartAsync(Compose);
    }

    public async Task DisposeAsync()
    {
        await _host.DisposeAsync();
        await Database.DropAsync(Clock, Ids);
    }

    private void Compose(WebApplicationBuilder builder)
    {
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"ConnectionStrings:{PersistenceRegistration.ConnectionStringName}"] = Database.ConnectionString,
            ["ResourceLock:ExpirySeconds"] = "30"
        });

        builder.Services
            .AddPersistence(builder.Configuration)
            .AddApplication(builder.Configuration);

        builder.Services.AddSingleton<IClock>(Clock);
        builder.Services.AddSingleton<IIdGenerator>(Ids);
        builder.Services.AddSingleton<IDistributedLock>(Locks);
    }
}
