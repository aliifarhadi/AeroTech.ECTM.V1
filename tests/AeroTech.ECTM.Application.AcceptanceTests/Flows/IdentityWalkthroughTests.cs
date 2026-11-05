using System.Net;
using System.Text.Json;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Flows;

[Collection(IdentityDatabaseCollection.Name)]
public sealed class IdentityWalkthroughTests(MigratedDatabaseFixture fixture)
{
    private const string Flight24 = "2026-10-01_EP-DOA_24.csv";
    private const string RenamedCopy = "2026-10-01_EP-XYZ_24.csv";

    private readonly ImportApi _imports = new(fixture);

    [Fact]
    public async Task J4_a_renamed_copy_with_an_unknown_registration_needs_identity_and_the_dialog_resolves_it()
    {
        var fleet = await _imports.EnsureFleetAsync();
        await _imports.ActivateSeedsAsync();

        var preview = await _imports.PreviewedAsync((RenamedCopy, ImportApi.RealBytes(Flight24)));
        var previewIdentity = ImportApi.ItemNamed(preview, RenamedCopy).GetProperty("identity");

        Assert.Equal("UNRESOLVED", ImportApi.NameOf(previewIdentity, "identityStatus"));
        Assert.Equal("EP-XYZ", previewIdentity.GetProperty("registration").GetString());
        Assert.Contains("AIRCRAFT_NOT_FOUND", previewIdentity.GetProperty("reasons").EnumerateArray().Select(reason => reason.GetString()));

        var batch = await _imports.CommittedAsync(FleetApi.IdOf(preview), "j4-commit");
        var item = ImportApi.ItemNamed(batch, RenamedCopy);
        var flightId = item.GetProperty("flightId").GetString()!;

        Assert.Equal("NEEDS_IDENTITY", ImportApi.NameOf(item, "flightState"));

        using (var wrongPosition = await ConfirmAsync(flightId, fleet.AircraftId, fleet.EngineTwoId, fleet.EngineOneId, confirmDistinct: false))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, wrongPosition.StatusCode);
            Assert.Equal(80312, await ApiResponses.FirstErrorCodeOf(wrongPosition));
        }

        using (var missingPosition = await _imports.PostAsync($"flights/{flightId}/identity", new
        {
            aircraftId = fleet.AircraftId,
            flightDate = "2026-10-01",
            flightNumber = "24",
            engines = new[] { new { position = 1, engineId = fleet.EngineOneId, positionOnly = false } },
            confirmDistinctFlight = false
        }))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, missingPosition.StatusCode);
            Assert.Equal(80309, await ApiResponses.FirstErrorCodeOf(missingPosition));
        }

        using (var resolved = await ConfirmAsync(flightId, fleet.AircraftId, fleet.EngineOneId, fleet.EngineTwoId, confirmDistinct: false))
        {
            Assert.Equal(HttpStatusCode.OK, resolved.StatusCode);

            var detail = await ApiResponses.DataOf(resolved);
            var identity = detail.GetProperty("identity");

            Assert.Equal("PENDING_REVIEW", ImportApi.NameOf(detail, "state"));
            Assert.Equal("RESOLVED", ImportApi.NameOf(identity, "identityStatus"));
            Assert.Equal("USER", ImportApi.NameOf(identity, "registrationSource"));
            Assert.Equal(ImportApi.Registration, identity.GetProperty("registration").GetString());
            Assert.Equal([ImportApi.EngineOneSerial, ImportApi.EngineTwoSerial], ImportApi.SerialsOf(identity));
        }

        await DifferentBytesWithTheSameIdentityNeedConfirmation(fleet, flightId);
    }

    private async Task DifferentBytesWithTheSameIdentityNeedConfirmation(ImportApi.FleetIds fleet, string firstFlightId)
    {
        var changed = ImportApi.WithLine(ImportApi.RealBytes(Flight24), 1, line => line + " ");
        var preview = await _imports.PreviewedAsync((Flight24, changed));
        var previewItem = ImportApi.ItemNamed(preview, Flight24);

        Assert.Equal("NEW", ImportApi.NameOf(previewItem, "outcome"));
        Assert.Contains("POSSIBLE_DUPLICATE_IDENTITY", previewItem.GetProperty("identity").GetProperty("reasons").EnumerateArray().Select(reason => reason.GetString()));

        var batch = await _imports.CommittedAsync(FleetApi.IdOf(preview), "possible-duplicate");
        var flightId = ImportApi.ItemNamed(batch, Flight24).GetProperty("flightId").GetString()!;

        Assert.NotEqual(firstFlightId, flightId);
        Assert.Equal("NEEDS_IDENTITY", ImportApi.NameOf(ImportApi.ItemNamed(batch, Flight24), "flightState"));

        using (var unconfirmed = await ConfirmAsync(flightId, fleet.AircraftId, fleet.EngineOneId, fleet.EngineTwoId, confirmDistinct: false))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, unconfirmed.StatusCode);
            Assert.Equal(80313, await ApiResponses.FirstErrorCodeOf(unconfirmed));
        }

        using var confirmed = await ConfirmAsync(flightId, fleet.AircraftId, fleet.EngineOneId, fleet.EngineTwoId, confirmDistinct: true);
        var detail = await ApiResponses.DataOf(confirmed);

        Assert.Equal("PENDING_REVIEW", ImportApi.NameOf(detail, "state"));
        Assert.True(detail.GetProperty("distinctFlightConfirmed").GetBoolean());
    }

    private Task<HttpResponseMessage> ConfirmAsync(string flightId, string aircraftId, string positionOneEngineId, string positionTwoEngineId, bool confirmDistinct)
        => _imports.PostAsync($"flights/{flightId}/identity", new
        {
            aircraftId,
            flightDate = "2026-10-01",
            flightNumber = "24",
            engines = new[]
            {
                new { position = 1, engineId = positionOneEngineId, positionOnly = false },
                new { position = 2, engineId = positionTwoEngineId, positionOnly = false }
            },
            confirmDistinctFlight = confirmDistinct,
            note = "checked against the technical log"
        });
}
