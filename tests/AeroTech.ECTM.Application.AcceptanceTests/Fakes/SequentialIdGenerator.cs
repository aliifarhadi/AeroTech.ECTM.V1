using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.AcceptanceTests.Fakes;

public sealed class SequentialIdGenerator : IIdGenerator
{
    private long _last = DateTime.UtcNow.Ticks;

    public long NewId() => Interlocked.Increment(ref _last);
}
