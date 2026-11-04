using AeroTech.ECTM.RestApi;
using AeroTech.ECTM.RestApi.Middlewares;
using AeroTech.Framework.Presentation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public sealed class RestApiHost : IAsyncDisposable
{
    private const string LoopbackOnAnyPort = "http://127.0.0.1:0";

    private readonly WebApplication _app;

    private RestApiHost(WebApplication app)
    {
        _app = app;
        Client = new HttpClient { BaseAddress = new Uri(app.Urls.Single()) };
    }

    public HttpClient Client { get; }

    public IServiceProvider Services => _app.Services;

    public static async Task<RestApiHost> StartAsync(Action<WebApplicationBuilder> compose)
    {
        var builder = WebApplication.CreateBuilder();

        builder.Logging.ClearProviders();
        builder.WebHost.UseUrls(LoopbackOnAnyPort);
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Authority"] = "https://identity.test/",
            ["Jwt:Audience"] = "ectm-test"
        });

        builder.Services.AddPresentation(builder.Configuration, typeof(RestApiAssembly).Assembly);

        compose(builder);

        var app = builder.Build();

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UsePresentation();
        await app.StartAsync();

        return new RestApiHost(app);
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _app.DisposeAsync();
    }
}
