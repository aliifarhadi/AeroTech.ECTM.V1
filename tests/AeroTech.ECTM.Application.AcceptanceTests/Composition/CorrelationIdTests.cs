using System.Net;
using System.Text.RegularExpressions;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using AeroTech.ECTM.RestApi.Middlewares;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Composition;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class CorrelationIdTests(MigratedDatabaseFixture fixture)
{
    private static readonly Regex TraceId = new("^[0-9a-f]{32}$");

    [Fact]
    public async Task A_successful_response_carries_a_correlation_id()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/kinds");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Matches(TraceId, CorrelationIdOf(response));
    }

    [Fact]
    public async Task An_error_response_carries_a_correlation_id()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/config/NOT_A_KIND/documents");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Matches(TraceId, CorrelationIdOf(response));
    }

    [Fact]
    public async Task Two_requests_carry_different_correlation_ids()
    {
        using var first = await fixture.Client.GetAsync("Backoffice/v1/config/kinds");
        using var second = await fixture.Client.GetAsync("Backoffice/v1/config/kinds");

        Assert.NotEqual(CorrelationIdOf(first), CorrelationIdOf(second));
    }

    private static string CorrelationIdOf(HttpResponseMessage response)
        => response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single();
}
