using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fleet;

public sealed class AircraftTests
{
    [Fact]
    public void A_registration_is_trimmed_and_upper_cased()
    {
        var aircraft = Aircraft.Register(1, " ep-doa ", "B737", null);

        Assert.Equal("EP-DOA", aircraft.Registration);
        Assert.Equal(2, aircraft.EngineCount);
        Assert.Equal(AircraftStatus.ACTIVE, aircraft.Status);
    }

    [Fact]
    public void A_blank_registration_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(() => Aircraft.Register(1, " ", null, null));

        Assert.Equal(80202, exception.Code);
    }
}
