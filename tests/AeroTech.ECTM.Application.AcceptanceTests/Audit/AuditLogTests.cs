using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fakes;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Audit;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class AuditLogTests(MigratedDatabaseFixture fixture)
{
    [Fact]
    public async Task Creating_a_document_writes_one_created_entry_with_actor_and_after_state()
    {
        var draft = await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.QUALITY_RULES, FixedActorResolver.Alice);

        using var response = await fixture.Client.GetAsync($"Backoffice/v1/audit?entityType=ConfigurationDocument&entityId={draft.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await ApiResponses.DataOf(response);
        var entry = page.GetProperty("results").EnumerateArray().Single();

        Assert.Equal("CREATED", entry.GetProperty("action").GetProperty("name").GetString());
        Assert.Equal("alice", entry.GetProperty("actorSubject").GetString());
        Assert.Equal("42", entry.GetProperty("actorId").GetString());
        Assert.Equal(fixture.Clock.Now, entry.GetProperty("occurredAt").GetDateTimeOffset());
        Assert.Equal("ConfigurationDocument", entry.GetProperty("entityType").GetString());
        Assert.Equal(draft.Id.ToString(), entry.GetProperty("entityId").GetString());
        Assert.Equal(JsonValueKind.Null, entry.GetProperty("before").ValueKind);

        var after = entry.GetProperty("after");
        Assert.Equal("QUALITY_RULES", after.GetProperty("Kind").GetString());
        Assert.Equal("DRAFT", after.GetProperty("Status").GetString());
        Assert.Equal(draft.ContentSha256, after.GetProperty("ContentSha256").GetString());
        Assert.False(after.TryGetProperty("RowVersion", out _));
    }

    [Fact]
    public async Task The_audit_log_is_filtered_by_actor()
    {
        var draft = await ConfigurationScenario.SeedDraftAsync(fixture, ConfigurationKind.QUALITY_RULES, FixedActorResolver.Alice);

        using var byAlice = await fixture.Client.GetAsync($"Backoffice/v1/audit?entityId={draft.Id}&actor=alice");
        using var byBob = await fixture.Client.GetAsync($"Backoffice/v1/audit?entityId={draft.Id}&actor=bob");

        Assert.Equal("1", (await ApiResponses.DataOf(byAlice)).GetProperty("totalCount").GetString());
        Assert.Equal("0", (await ApiResponses.DataOf(byBob)).GetProperty("totalCount").GetString());
    }

    [Fact]
    public async Task A_period_that_ends_before_it_starts_is_rejected()
    {
        using var response = await fixture.Client.GetAsync("Backoffice/v1/audit?from=2026-11-05T00:00:00Z&to=2026-11-04T00:00:00Z");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
