using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.ConfigurationDocuments;

public sealed class ConfigurationDocumentTests
{
    private const string Content = """{"a":1}""";
    private const string ContentSha256ComputedOutsideTheProduct = "015abd7f5cc57a2dd94b7590f04ad8084273905ee33ec5cebeae62276a97f862";

    [Fact]
    public void A_draft_carries_its_kind_version_status_and_content_hash()
    {
        var document = ConfigurationDocument.CreateDraft(7, ConfigurationKind.DATA_PROFILE, 1, null, Content);

        Assert.Equal(7, document.Id);
        Assert.Equal(ConfigurationKind.DATA_PROFILE, document.Kind);
        Assert.Equal(1, document.VersionNumber);
        Assert.Equal(ConfigurationDocumentStatus.DRAFT, document.Status);
        Assert.Null(document.BasedOnDocumentId);
        Assert.Equal(Content, document.Content);
        Assert.Equal(ContentSha256ComputedOutsideTheProduct, document.ContentSha256);
    }

    [Fact]
    public void A_draft_keeps_the_document_it_is_based_on()
    {
        var document = ConfigurationDocument.CreateDraft(8, ConfigurationKind.QUALITY_RULES, 2, 7, Content);

        Assert.Equal(7, document.BasedOnDocumentId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void A_version_below_one_is_refused(int versionNumber)
    {
        var exception = Assert.Throws<BusinessException>(
            () => ConfigurationDocument.CreateDraft(1, ConfigurationKind.DATA_PROFILE, versionNumber, null, Content));

        Assert.Equal(80102, exception.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_content_is_refused(string content)
    {
        var exception = Assert.Throws<BusinessException>(
            () => ConfigurationDocument.CreateDraft(1, ConfigurationKind.DATA_PROFILE, 1, null, content));

        Assert.Equal(80103, exception.Code);
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("[1,2]")]
    [InlineData("42")]
    [InlineData("{\"a\":")]
    public void Content_that_is_not_one_json_object_is_refused(string content)
    {
        var exception = Assert.Throws<BusinessException>(
            () => ConfigurationDocument.CreateDraft(1, ConfigurationKind.DATA_PROFILE, 1, null, content));

        Assert.Equal(80104, exception.Code);
    }

    [Fact]
    public void An_undefined_kind_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(
            () => ConfigurationDocument.CreateDraft(1, (ConfigurationKind)99, 1, null, Content));

        Assert.Equal(80100, exception.Code);
    }
}
