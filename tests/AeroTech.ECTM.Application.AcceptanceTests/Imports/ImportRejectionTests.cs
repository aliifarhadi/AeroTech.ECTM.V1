using System.Net;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Imports;

[Collection(ImportDatabaseCollection.Name)]
public sealed class ImportRejectionTests(MigratedDatabaseFixture fixture)
{
    private const string Flight24 = "2026-10-01_EP-DOA_24.csv";

    private readonly ImportApi _imports = new(fixture);

    [Fact]
    public async Task A_synthetic_source_is_rejected_outside_a_test_environment()
    {
        await _imports.ActivateSeedsAsync();
        var name = $"SYNTHETIC_TEST_ONLY_{Flight24}";

        var preview = await _imports.PreviewedAsync((name, ImportApi.RealBytes(Flight24)));
        var item = ImportApi.ItemNamed(preview, name);

        Assert.Equal("REJECTED", ImportApi.NameOf(item, "outcome"));
        Assert.Equal("SYNTHETIC_SOURCE_NOT_ALLOWED", item.GetProperty("rejectionCode").GetString());
    }

    [Fact]
    public async Task Swapped_columns_are_rejected_with_the_mismatches_and_no_flight_is_created()
    {
        await _imports.ActivateSeedsAsync();
        var swapped = ImportApi.WithLine(ImportApi.RealBytes(Flight24), 0, header =>
        {
            var cells = header.Split(',');
            (cells[13], cells[14]) = (cells[14], cells[13]);
            return string.Join(',', cells);
        });

        var preview = await _imports.PreviewedAsync((Flight24, swapped));
        var item = ImportApi.ItemNamed(preview, Flight24);

        Assert.Equal("REJECTED", ImportApi.NameOf(item, "outcome"));
        Assert.Equal("PROFILE_SIGNATURE_MISMATCH", item.GetProperty("rejectionCode").GetString());
        Assert.Equal(2, item.GetProperty("headerMismatches").GetArrayLength());

        var committed = await _imports.CommittedAsync(FleetApi.IdOf(preview), "swapped-columns");

        Assert.Equal("SKIPPED", ImportApi.NameOf(ImportApi.ItemNamed(committed, Flight24), "state"));
        Assert.Equal(JsonValueKindNull, ImportApi.ItemNamed(committed, Flight24).GetProperty("flightId").ValueKind);
    }

    [Fact]
    public async Task An_unsupported_member_of_an_archive_is_rejected_on_its_own()
    {
        await _imports.ActivateSeedsAsync();
        var archive = ImportApi.Zip([("manual.pdf", [0x25, 0x50, 0x44, 0x46]), ("folder/notes.txt", "x"u8.ToArray())]);

        var preview = await _imports.PreviewedAsync(("mixed.zip", archive));

        Assert.All(preview.GetProperty("items").EnumerateArray(), item =>
        {
            Assert.Equal("REJECTED", ImportApi.NameOf(item, "outcome"));
            Assert.Equal("UNSUPPORTED_FILE_TYPE", item.GetProperty("rejectionCode").GetString());
        });
        Assert.Equal(["manual.pdf", "notes.txt"], preview.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("fileName").GetString()));
    }

    [Theory]
    [InlineData("notes.pdf", 80301)]
    [InlineData("broken.zip", 80303)]
    public async Task An_upload_that_cannot_be_expanded_is_refused(string fileName, int errorCode)
    {
        await _imports.ActivateSeedsAsync();

        using var response = await _imports.PreviewAsync([(fileName, "not an archive"u8.ToArray())]);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal(errorCode, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task An_upload_without_a_file_is_a_bad_request()
    {
        using var response = await _imports.PreviewAsync([]);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("imports/987654321", 80300)]
    [InlineData("jobs/987654321", 80310)]
    [InlineData("flights/987654321", 80311)]
    public async Task An_unknown_resource_is_not_found(string route, int errorCode)
    {
        using var response = await _imports.GetAsync(route);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(errorCode, await ApiResponses.FirstErrorCodeOf(response));
    }

    private const System.Text.Json.JsonValueKind JsonValueKindNull = System.Text.Json.JsonValueKind.Null;
}
