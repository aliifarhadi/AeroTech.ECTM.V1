using AeroTech.ECTM.Domain.MaintenanceEventAggregate;
using AeroTech.ECTM.Domain.MaintenanceEventAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fleet;

public sealed class MaintenanceEventTests
{
    private static readonly MaintenanceEventArgs Wash = new(
        MaintenanceEventType.WASH,
        new DateTimeOffset(2026, 10, 5, 0, 0, 0, TimeSpan.Zero),
        DateReliability.DAY_ONLY,
        12400m,
        6800,
        "WO-1",
        null);

    [Fact]
    public void An_engine_event_names_only_the_engine()
    {
        var maintenanceEvent = MaintenanceEvent.ForEngine(1, 7, Wash);

        Assert.Equal(IdentityKind.ENGINE, maintenanceEvent.IdentityKind);
        Assert.Equal(7, maintenanceEvent.EngineId);
        Assert.Null(maintenanceEvent.AircraftId);
        Assert.Null(maintenanceEvent.Position);
    }

    [Fact]
    public void A_position_only_event_names_the_aircraft_position_and_no_engine()
    {
        var maintenanceEvent = MaintenanceEvent.ForPosition(1, 10, 2, Wash);

        Assert.Equal(IdentityKind.POSITION_ONLY, maintenanceEvent.IdentityKind);
        Assert.Null(maintenanceEvent.EngineId);
        Assert.Equal(10, maintenanceEvent.AircraftId);
        Assert.Equal((byte)2, maintenanceEvent.Position);
    }

    [Fact]
    public void A_position_only_event_on_position_three_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(() => MaintenanceEvent.ForPosition(1, 10, 3, Wash));

        Assert.Equal(80209, exception.Code);
    }

    [Fact]
    public void Negative_hours_or_cycles_are_refused()
    {
        var negativeHours = Assert.Throws<BusinessException>(() => MaintenanceEvent.ForEngine(1, 7, Wash with { HoursAtEvent = -1 }));
        var negativeCycles = Assert.Throws<BusinessException>(() => MaintenanceEvent.ForEngine(1, 7, Wash with { CyclesAtEvent = -1 }));

        Assert.Equal(80217, negativeHours.Code);
        Assert.Equal(80217, negativeCycles.Code);
    }

    [Fact]
    public void An_event_is_a_marker_that_keeps_its_supplied_values()
    {
        var maintenanceEvent = MaintenanceEvent.ForEngine(1, 7, Wash);

        Assert.Equal(MaintenanceEventType.WASH, maintenanceEvent.EventType);
        Assert.Equal(DateReliability.DAY_ONLY, maintenanceEvent.DateReliability);
        Assert.Equal(12400m, maintenanceEvent.HoursAtEvent);
        Assert.Equal(6800, maintenanceEvent.CyclesAtEvent);
        Assert.Equal("WO-1", maintenanceEvent.WorkOrderReference);
    }
}
