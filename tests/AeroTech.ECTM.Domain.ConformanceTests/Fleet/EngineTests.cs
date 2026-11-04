using AeroTech.ECTM.Domain.EngineAggregate;
using AeroTech.ECTM.Domain.EngineAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Fleet;

public sealed class EngineTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 5, 8, 0, 0, TimeSpan.Zero);
    private static readonly EngineConfigurationArgs NoConfiguration = new(null, null, null);
    private static readonly LifeCounterArgs Counters = new(12345.5m, 6789, 120.25m, 80, new DateTimeOffset(2026, 9, 24, 0, 0, 0, TimeSpan.Zero));

    [Fact]
    public void A_documented_variant_without_evidence_is_refused()
    {
        var configuration = new EngineConfigurationArgs("-3B1", null, null);

        var exception = Assert.Throws<BusinessException>(
            () => Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, configuration, "alice", Now));

        Assert.Equal(80213, exception.Code);
    }

    [Fact]
    public void Documented_configuration_is_stamped_with_actor_and_time()
    {
        var configuration = new EngineConfigurationArgs("-3B1", "B1", "engine log book page 12");

        var engine = Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, configuration, "alice", Now);

        Assert.Equal("-3B1", engine.VariantCode);
        Assert.Equal("B1", engine.ThrustRatingCode);
        Assert.Equal("engine log book page 12", engine.ConfigurationEvidenceReference);
        Assert.Equal(Now, engine.ConfigurationDocumentedAt);
        Assert.Equal("alice", engine.ConfigurationDocumentedBy);
    }

    [Fact]
    public void An_unchanged_configuration_keeps_its_original_stamp()
    {
        var configuration = new EngineConfigurationArgs("-3B1", "B1", "engine log book page 12");
        var engine = Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, configuration, "alice", Now);

        engine.UpdateDetails(EngineStatus.STORED, configuration, "bob", Now.AddDays(1));

        Assert.Equal(EngineStatus.STORED, engine.Status);
        Assert.Equal(Now, engine.ConfigurationDocumentedAt);
        Assert.Equal("alice", engine.ConfigurationDocumentedBy);
    }

    [Fact]
    public void A_blank_serial_number_is_refused()
    {
        var exception = Assert.Throws<BusinessException>(
            () => Engine.Register(1, 2, "CFMI", "  ", EngineStatus.ACTIVE, NoConfiguration, null, Now));

        Assert.Equal(80208, exception.Code);
    }

    [Fact]
    public void Every_counter_setting_appends_one_adjustment_with_its_reason()
    {
        var engine = Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, NoConfiguration, "alice", Now);

        engine.AdjustLifeCounters(10, Counters, Engine.InitialLifeCountersReason, "alice", Now);
        engine.AdjustLifeCounters(11, Counters with { CyclesSinceNew = 6790 }, "logbook correction", "bob", Now.AddHours(1));

        Assert.Equal(2, engine.LifeCounterAdjustments.Count);
        Assert.Equal(6790, engine.CyclesSinceNew);
        Assert.Equal(LifeCounterSource.MANUAL, engine.LifeCountersSource);

        var correction = engine.LifeCounterAdjustments.Last();
        Assert.Equal("logbook correction", correction.Reason);
        Assert.Equal("bob", correction.AdjustedBy);
        Assert.Equal(6790, correction.CyclesSinceNew);
    }

    [Fact]
    public void A_negative_counter_is_refused()
    {
        var engine = Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, NoConfiguration, null, Now);

        var exception = Assert.Throws<BusinessException>(
            () => engine.AdjustLifeCounters(10, Counters with { HoursSinceNew = -0.1m }, "typo", null, Now));

        Assert.Equal(80214, exception.Code);
        Assert.Empty(engine.LifeCounterAdjustments);
    }

    [Fact]
    public void A_counter_adjustment_without_a_reason_is_refused()
    {
        var engine = Engine.Register(1, 2, "CFMI", "Eng1S1", EngineStatus.ACTIVE, NoConfiguration, null, Now);

        var exception = Assert.Throws<BusinessException>(() => engine.AdjustLifeCounters(10, Counters, "", null, Now));

        Assert.Equal(80220, exception.Code);
    }
}
