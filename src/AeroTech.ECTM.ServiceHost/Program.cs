using AeroTech.ECTM.Application;
using AeroTech.ECTM.Consumers;
using AeroTech.ECTM.Persistence;
using AeroTech.ECTM.RestApi;
using AeroTech.ECTM.RestApi.Middlewares;
using AeroTech.ECTM.RestApi.Swagger;
using AeroTech.Framework.Infrastructure;
using AeroTech.Framework.Presentation.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext().WriteTo.Console());

builder.Services
    .AddFrameworkInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddConsumers(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddPresentation(builder.Configuration, typeof(RestApiAssembly).Assembly);

builder.Services.ConfigureOptions<EctmSwaggerGenOptions>();
builder.Services.ConfigureOptions<EctmSwaggerUIOptions>();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AeroTech ECTM API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UsePresentation();

app.Run();

public partial class Program
{
}
