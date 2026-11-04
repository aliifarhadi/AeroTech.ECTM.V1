using AeroTech.ECTM.Domain.ConformanceTests.Fakes;
using AeroTech.ECTM.Domain.EngineInstallationAggregate;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fleet;

public sealed class EngineInstallationTests
{
    private const long EngineOne = 1;
    private const long EngineTwo = 2;
    private const long AircraftA = 10;
    private const long AircraftB = 20;

    private static readonly DateTimeOffset Now = new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset September24 = new(2026, 9, 24, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset October1 = new(2026, 10, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly SequenceIds _ids = new();

    [Fact]
    public void An_installation_records_an_installed_change_with_its_reason()
    {
        var installation = Install(EngineOne, AircraftA, 1, September24);

        var change = Assert.Single(installation.Changes);
        Assert.Equal(RegistryChangeType.INSTALLED, change.ChangeType);
        Assert.Equal("initial installation", change.Reason);
        Assert.True(installation.IsOpen);
    }

    [Fact]
    public void Another_engine_on_the_same_open_position_overlaps()
    {
        var existing = Install(EngineOne, AircraftA, 1, September24);
        var candidate = Install(EngineTwo, AircraftA, 1, October1);

        var conflict = candidate.FindOverlapIn([existing]);

        Assert.Same(existing, conflict);
    }

    [Fact]
    public void The_same_engine_on_another_aircraft_overlaps()
    {
        var existing = Install(EngineOne, AircraftA, 1, September24);
        var candidate = Install(EngineOne, AircraftB, 2, October1);

        var conflict = candidate.FindOverlapIn([existing]);

        Assert.Same(existing, conflict);
    }

    [Fact]
    public void A_swap_at_the_removal_instant_does_not_overlap()
    {
        var existing = Install(EngineOne, AircraftA, 1, September24);
        existing.Close(October1, "removed for shop visit", _ids, Now);

        var replacement = Install(EngineTwo, AircraftA, 1, October1);

        Assert.Null(replacement.FindOverlapIn([existing]));
    }

    [Fact]
    public void An_installation_one_second_before_the_removal_overlaps()
    {
        var existing = Install(EngineOne, AircraftA, 1, September24);
        existing.Close(October1, "removed for shop visit", _ids, Now);

        var replacement = Install(EngineTwo, AircraftA, 1, October1.AddSeconds(-1));

        Assert.Same(existing, replacement.FindOverlapIn([existing]));
    }

    [Fact]
    public void An_earlier_closed_period_before_an_existing_one_does_not_overlap()
    {
        var existing = Install(EngineOne, AircraftA, 1, October1);

        var earlier = Install(EngineTwo, AircraftA, 1, September24);
        earlier.Close(October1, "removed", _ids, Now);

        Assert.Null(earlier.FindOverlapIn([existing]));
    }

    [Fact]
    public void Installations_of_other_engines_on_other_positions_never_overlap()
    {
        var existing = Install(EngineOne, AircraftA, 1, September24);
        var candidate = Install(EngineTwo, AircraftA, 2, September24);

        Assert.Null(candidate.FindOverlapIn([existing]));
    }

    [Fact]
    public void Closing_records_a_removed_change_and_ends_the_period()
    {
        var installation = Install(EngineOne, AircraftA, 1, September24);

        installation.Close(October1, "removed for shop visit", _ids, Now);

        Assert.False(installation.IsOpen);
        Assert.Equal(October1, installation.RemovedAt);
        Assert.Equal("removed for shop visit", installation.RemovalReason);
        Assert.Equal(RegistryChangeType.REMOVED, installation.Changes.Last().ChangeType);
    }

    [Fact]
    public void A_removal_not_after_the_installation_is_refused()
    {
        var installation = Install(EngineOne, AircraftA, 1, September24);

        var exception = Assert.Throws<BusinessException>(() => installation.Close(September24, "removed", _ids, Now));

        Assert.Equal(80212, exception.Code);
    }

    [Fact]
    public void Closing_twice_is_refused()
    {
        var installation = Install(EngineOne, AircraftA, 1, September24);
        installation.Close(October1, "removed", _ids, Now);

        var exception = Assert.Throws<BusinessException>(() => installation.Close(October1.AddDays(1), "removed again", _ids, Now));

        Assert.Equal(80215, exception.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void A_position_other_than_one_or_two_is_refused(int position)
    {
        var exception = Assert.Throws<BusinessException>(() => Install(EngineOne, AircraftA, position, September24));

        Assert.Equal(80209, exception.Code);
    }

    [Fact]
    public void A_blank_reason_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(
            () => EngineInstallation.Record(_ids.NewId(), EngineOne, AircraftA, 1, September24, " ", null, _ids, Now));

        Assert.Equal(80219, exception.Code);
    }

    [Fact]
    public void The_installation_time_is_stored_in_utc()
    {
        var tehranTime = new DateTimeOffset(2026, 9, 24, 3, 30, 0, TimeSpan.FromHours(3.5));

        var installation = Install(EngineOne, AircraftA, 1, tehranTime);

        Assert.Equal(TimeSpan.Zero, installation.InstalledAt.Offset);
        Assert.Equal(September24, installation.InstalledAt);
    }

    private EngineInstallation Install(long engineId, long aircraftId, int position, DateTimeOffset installedAt)
        => EngineInstallation.Record(_ids.NewId(), engineId, aircraftId, position, installedAt, "initial installation", null, _ids, Now);
}
