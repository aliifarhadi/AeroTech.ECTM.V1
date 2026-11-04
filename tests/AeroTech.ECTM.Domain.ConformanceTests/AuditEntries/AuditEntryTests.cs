using AeroTech.ECTM.Domain.AuditEntryAggregate;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.AuditEntries;

public sealed class AuditEntryTests
{
    [Fact]
    public void The_time_of_an_entry_is_stored_in_utc()
    {
        var tehranTime = new DateTimeOffset(2026, 11, 5, 10, 30, 0, TimeSpan.FromHours(3.5));

        var entry = AuditEntry.Record(1, tehranTime, "alice", 42, "ConfigurationDocument", 9, AuditAction.CREATED, null, "{}", null);

        Assert.Equal(TimeSpan.Zero, entry.OccurredAt.Offset);
        Assert.Equal(new DateTimeOffset(2026, 11, 5, 7, 0, 0, TimeSpan.Zero), entry.OccurredAt);
    }
}
