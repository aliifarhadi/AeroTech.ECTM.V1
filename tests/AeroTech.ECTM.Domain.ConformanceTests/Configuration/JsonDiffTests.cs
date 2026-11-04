using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Configuration;

public sealed class JsonDiffTests
{
    [Fact]
    public void Identical_documents_have_no_change()
    {
        Assert.Empty(JsonDiff.Compare("""{"a":1,"b":[1,2]}""", """{"b":[1,2],"a":1.0}"""));
    }

    [Fact]
    public void Changed_added_and_removed_values_are_listed_with_their_paths()
    {
        var changes = JsonDiff.Compare(
            """{"a":1,"b":{"c":"x"},"list":[1,2,3],"gone":true}""",
            """{"a":2,"b":{"c":"y"},"list":[1,2],"new":null}""");

        Assert.Equal(
            [
                new JsonChange("$.a", JsonDiff.Changed, "1", "2"),
                new JsonChange("$.b.c", JsonDiff.Changed, "\"x\"", "\"y\""),
                new JsonChange("$.list[2]", JsonDiff.Removed, "3", null),
                new JsonChange("$.gone", JsonDiff.Removed, "true", null),
                new JsonChange("$.new", JsonDiff.Added, null, "null")
            ],
            changes);
    }
}
