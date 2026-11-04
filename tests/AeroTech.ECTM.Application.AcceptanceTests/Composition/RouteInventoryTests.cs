using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Composition;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class RouteInventoryTests(MigratedDatabaseFixture fixture)
{
    private const string BusinessRoutePrefix = "Backoffice/";

    private static readonly string[] P00Routes =
    [
        "GET Backoffice/v{version:apiVersion}/audit",
        "GET Backoffice/v{version:apiVersion}/config/kinds",
        "GET Backoffice/v{version:apiVersion}/config/{kind}/documents",
        "GET Backoffice/v{version:apiVersion}/config/{kind}/documents/{documentId:long}"
    ];

    [Fact]
    public void The_business_routes_are_exactly_the_P00_contract()
    {
        var routes = BusinessRoutes();

        Assert.Equal(P00Routes, routes);
    }

    private List<string> BusinessRoutes()
    {
        var endpoints = fixture.Services.GetRequiredService<EndpointDataSource>().Endpoints;
        var routes = new List<string>();

        foreach (var endpoint in endpoints.OfType<RouteEndpoint>())
        {
            var pattern = endpoint.RoutePattern.RawText ?? string.Empty;

            if (!pattern.StartsWith(BusinessRoutePrefix, StringComparison.Ordinal))
                continue;

            var methods = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods ?? [];

            foreach (var method in methods)
                routes.Add($"{method} {pattern}");
        }

        return routes.Order(StringComparer.Ordinal).ToList();
    }
}
