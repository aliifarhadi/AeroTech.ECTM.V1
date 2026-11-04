using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;
using Microsoft.EntityFrameworkCore;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fixtures;

public static class ConfigurationScenario
{
    public const string DraftContent = """{"purpose":"acceptance test draft"}""";

    public static async Task<ConfigurationDocument> SeedDraftAsync(MigratedDatabaseFixture fixture, ConfigurationKind kind, IActorResolver actorResolver)
    {
        await using var context = fixture.NewContext(actorResolver);

        var lastVersion = await context.ConfigurationDocuments
            .Where(document => document.Kind == kind)
            .MaxAsync(document => (int?)document.VersionNumber);
        var nextVersion = (lastVersion ?? 0) + 1;

        var draft = ConfigurationDocument.CreateDraft(fixture.Ids.NewId(), kind, nextVersion, null, DraftContent);

        context.ConfigurationDocuments.Add(draft);
        await context.SaveChangesAsync();

        return draft;
    }
}
