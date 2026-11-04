using System.Net;
using System.Text;
using AeroTech.ECTM.Application.AcceptanceTests.Fixtures;
using Xunit;

namespace AeroTech.ECTM.Application.AcceptanceTests.Configuration;

[Collection(MigratedDatabaseCollection.Name)]
public sealed class ProfileDryRunTests(MigratedDatabaseFixture fixture)
{
    private const string Flight24 = "2026-10-01_EP-DOA_24.csv";

    private readonly ConfigurationApi _configuration = new(fixture.Client);

    [Fact]
    public async Task The_active_profile_previews_a_real_recording()
    {
        var profileId = await ActiveProfileAsync();

        using var response = await _configuration.PostFileAsync($"config/{ConfigurationApi.Profile}/documents/{profileId}/dry-run", Flight24, Recording());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var preview = await ApiResponses.DataOf(response);
        Assert.True(preview.GetProperty("signatureMatches").GetBoolean());
        Assert.Equal(3987, preview.GetProperty("dataRowCount").GetInt32());
        Assert.Equal(10, preview.GetProperty("firstRows").GetArrayLength());
        Assert.Equal(10, preview.GetProperty("lastRows").GetArrayLength());
        Assert.Equal("ae6290241c5a2a2ed2ca11cfcdfc0e59417109433e65220ebabdf0b22adc8718", preview.GetProperty("fileSha256").GetString());
    }

    [Fact]
    public async Task A_recording_with_two_swapped_columns_is_a_mismatch()
    {
        var profileId = await ActiveProfileAsync();
        var text = Encoding.UTF8.GetString(Recording());
        var lineEnd = text.IndexOf('\n');
        var header = text[..lineEnd].Split(',');
        (header[15], header[16]) = (header[16], header[15]);
        var swapped = Encoding.UTF8.GetBytes(string.Join(',', header) + text[lineEnd..]);

        using var response = await _configuration.PostFileAsync($"config/{ConfigurationApi.Profile}/documents/{profileId}/dry-run", "swapped.csv", swapped);

        var preview = await ApiResponses.DataOf(response);
        Assert.False(preview.GetProperty("signatureMatches").GetBoolean());
        Assert.Equal([15, 16], preview.GetProperty("headerMismatches").EnumerateArray().Select(mismatch => mismatch.GetProperty("ordinal").GetInt32()));
        Assert.Equal(0, preview.GetProperty("firstRows").GetArrayLength());
    }

    [Fact]
    public async Task A_file_that_is_not_utf8_is_unreadable()
    {
        var profileId = await ActiveProfileAsync();

        using var response = await _configuration.PostFileAsync($"config/{ConfigurationApi.Profile}/documents/{profileId}/dry-run", "broken.csv", [0x41, 0xFF, 0xFE, 0x0A]);

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80112, await ApiResponses.FirstErrorCodeOf(response));
    }

    [Fact]
    public async Task A_catalog_has_no_file_dry_run()
    {
        var catalogId = await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);

        using var response = await _configuration.PostFileAsync($"config/{ConfigurationApi.Catalog}/documents/{catalogId}/dry-run", Flight24, Recording());

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(80111, await ApiResponses.FirstErrorCodeOf(response));
    }

    private async Task<string> ActiveProfileAsync()
    {
        await _configuration.EnsureActiveAsync(ConfigurationApi.Catalog);

        return await _configuration.EnsureActiveAsync(ConfigurationApi.Profile);
    }

    private static byte[] Recording() => File.ReadAllBytes(RepositoryPaths.Fixtures("real", Flight24));
}
