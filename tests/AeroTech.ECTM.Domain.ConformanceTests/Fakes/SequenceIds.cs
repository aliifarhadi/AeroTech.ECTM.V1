using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fakes;

public sealed class SequenceIds : IIdGenerator
{
    private long _last = 1000;

    public long NewId() => ++_last;
}
