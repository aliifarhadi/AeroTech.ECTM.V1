using AeroTech.ECTM.Domain.EngineAggregate.Arguments;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.EngineAggregate.Entities
{
    public sealed class LifeCounterAdjustment : Entity<long>
    {
        private LifeCounterAdjustment()
        {
        }

        internal LifeCounterAdjustment(
            long id,
            long engineId,
            LifeCounterArgs counters,
            LifeCounterSource source,
            string reason,
            DateTimeOffset adjustedAt,
            string? adjustedBy)
        {
            Id = id;
            EngineId = engineId;
            HoursSinceNew = counters.HoursSinceNew;
            CyclesSinceNew = counters.CyclesSinceNew;
            HoursSinceLastEvent = counters.HoursSinceLastEvent;
            CyclesSinceLastEvent = counters.CyclesSinceLastEvent;
            BasisAt = counters.BasisAt.ToUniversalTime();
            Source = source;
            Reason = reason;
            AdjustedAt = adjustedAt;
            AdjustedBy = adjustedBy;
        }

        public long EngineId { get; private set; }

        public decimal? HoursSinceNew { get; private set; }

        public int? CyclesSinceNew { get; private set; }

        public decimal? HoursSinceLastEvent { get; private set; }

        public int? CyclesSinceLastEvent { get; private set; }

        public DateTimeOffset BasisAt { get; private set; }

        public LifeCounterSource Source { get; private set; }

        public string Reason { get; private set; } = default!;

        public DateTimeOffset AdjustedAt { get; private set; }

        public string? AdjustedBy { get; private set; }
    }
}
