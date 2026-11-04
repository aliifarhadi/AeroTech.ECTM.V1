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

    private static readonly string[] ContractRoutes =
    [
        "GET Backoffice/v{version:apiVersion}/aircraft",
        "GET Backoffice/v{version:apiVersion}/aircraft/{aircraftId:long}",
        "GET Backoffice/v{version:apiVersion}/audit",
        "GET Backoffice/v{version:apiVersion}/config/kinds",
        "GET Backoffice/v{version:apiVersion}/config/{kind}/documents",
        "GET Backoffice/v{version:apiVersion}/config/{kind}/documents/{documentId:long}",
        "GET Backoffice/v{version:apiVersion}/engine-models",
        "GET Backoffice/v{version:apiVersion}/engine-models/{engineModelId:long}",
        "GET Backoffice/v{version:apiVersion}/engines",
        "GET Backoffice/v{version:apiVersion}/engines/{engineId:long}",
        "GET Backoffice/v{version:apiVersion}/installations",
        "GET Backoffice/v{version:apiVersion}/maintenance-events",
        "POST Backoffice/v{version:apiVersion}/aircraft",
        "POST Backoffice/v{version:apiVersion}/engine-models",
        "POST Backoffice/v{version:apiVersion}/engines",
        "POST Backoffice/v{version:apiVersion}/installations",
        "POST Backoffice/v{version:apiVersion}/installations/{installationId:long}/close",
        "POST Backoffice/v{version:apiVersion}/maintenance-events",
        "PUT Backoffice/v{version:apiVersion}/aircraft/{aircraftId:long}",
        "PUT Backoffice/v{version:apiVersion}/engine-models/{engineModelId:long}",
        "PUT Backoffice/v{version:apiVersion}/engines/{engineId:long}",
        "PUT Backoffice/v{version:apiVersion}/engines/{engineId:long}/life-counters",
        "PUT Backoffice/v{version:apiVersion}/maintenance-events/{maintenanceEventId:long}"
    ];

    [Fact]
    public void The_business_routes_are_exactly_the_contract_of_the_delivered_phases()
    {
        var routes = BusinessRoutes();

        Assert.Equal(ContractRoutes, routes);
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
