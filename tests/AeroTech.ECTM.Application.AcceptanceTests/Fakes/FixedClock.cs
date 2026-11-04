using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fakes;

public sealed class FixedClock : IClock
{
    public DateTimeOffset Now { get; set; } = new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero);

    public DateTimeOffset GetDateTime() => Now;

    public DateOnly GetDate() => DateOnly.FromDateTime(Now.UtcDateTime);
}
