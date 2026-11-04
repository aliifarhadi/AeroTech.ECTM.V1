using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.EngineAggregate.Arguments;
using AeroTech.ECTM.Domain.EngineAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.EngineAggregate
{
    public sealed class Engine : AggregateRoot<long>
    {
        public const string InitialLifeCountersReason = "Initial life counters at registration";

        private readonly List<LifeCounterAdjustment> _lifeCounterAdjustments = new();

        private Engine()
        {
        }

        private Engine(long id, long engineModelId, string manufacturer, string serialNumber, EngineStatus status)
        {
            Id = id;
            EngineModelId = engineModelId;
            Manufacturer = manufacturer;
            SerialNumber = serialNumber;
            Status = status;
        }

        public long EngineModelId { get; private set; }

        public string Manufacturer { get; private set; } = default!;

        public string SerialNumber { get; private set; } = default!;

        public EngineStatus Status { get; private set; }

        public string? VariantCode { get; private set; }

        public string? ThrustRatingCode { get; private set; }

        public string? ConfigurationEvidenceReference { get; private set; }

        public DateTimeOffset? ConfigurationDocumentedAt { get; private set; }

        public string? ConfigurationDocumentedBy { get; private set; }

        public decimal? HoursSinceNew { get; private set; }

        public int? CyclesSinceNew { get; private set; }

        public decimal? HoursSinceLastEvent { get; private set; }

        public int? CyclesSinceLastEvent { get; private set; }

        public DateTimeOffset? LifeCountersBasisAt { get; private set; }

        public LifeCounterSource? LifeCountersSource { get; private set; }

        public IReadOnlyCollection<LifeCounterAdjustment> LifeCounterAdjustments => _lifeCounterAdjustments.AsReadOnly();

        public static Engine Register(
            long id,
            long engineModelId,
            string manufacturer,
            string serialNumber,
            EngineStatus status,
            EngineConfigurationArgs configuration,
            string? actor,
            DateTimeOffset now)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                throw ExceptionFactory.EngineSerialIsRequired();

            var engine = new Engine(id, engineModelId, manufacturer.Trim(), serialNumber.Trim(), status);
            engine.DocumentConfiguration(configuration, actor, now);

            return engine;
        }

        public void UpdateDetails(EngineStatus status, EngineConfigurationArgs configuration, string? actor, DateTimeOffset now)
        {
            Status = status;
            DocumentConfiguration(configuration, actor, now);
        }

        public void AdjustLifeCounters(long adjustmentId, LifeCounterArgs counters, string reason, string? actor, DateTimeOffset now)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw ExceptionFactory.LifeCounterAdjustmentReasonIsRequired();

            EnsureNotNegative(counters);

            HoursSinceNew = counters.HoursSinceNew;
            CyclesSinceNew = counters.CyclesSinceNew;
            HoursSinceLastEvent = counters.HoursSinceLastEvent;
            CyclesSinceLastEvent = counters.CyclesSinceLastEvent;
            LifeCountersBasisAt = counters.BasisAt.ToUniversalTime();
            LifeCountersSource = LifeCounterSource.MANUAL;

            var adjustment = new LifeCounterAdjustment(adjustmentId, Id, counters, LifeCounterSource.MANUAL, reason.Trim(), now, actor);
            _lifeCounterAdjustments.Add(adjustment);
        }

        private void DocumentConfiguration(EngineConfigurationArgs configuration, string? actor, DateTimeOffset now)
        {
            var variantCode = OptionalText.Normalize(configuration.VariantCode);
            var thrustRatingCode = OptionalText.Normalize(configuration.ThrustRatingCode);
            var evidenceReference = OptionalText.Normalize(configuration.EvidenceReference);

            var documentsAValue = variantCode is not null || thrustRatingCode is not null;

            if (documentsAValue && evidenceReference is null)
                throw ExceptionFactory.EngineConfigurationEvidenceIsRequired();

            var changed = variantCode != VariantCode
                          || thrustRatingCode != ThrustRatingCode
                          || evidenceReference != ConfigurationEvidenceReference;

            if (!changed)
                return;

            VariantCode = variantCode;
            ThrustRatingCode = thrustRatingCode;
            ConfigurationEvidenceReference = evidenceReference;

            if (evidenceReference is null)
            {
                ConfigurationDocumentedAt = null;
                ConfigurationDocumentedBy = null;
                return;
            }

            ConfigurationDocumentedAt = now;
            ConfigurationDocumentedBy = actor;
        }

        private static void EnsureNotNegative(LifeCounterArgs counters)
        {
            if (counters.HoursSinceNew < 0)
                throw ExceptionFactory.LifeCountersAreInvalid();

            if (counters.CyclesSinceNew < 0)
                throw ExceptionFactory.LifeCountersAreInvalid();

            if (counters.HoursSinceLastEvent < 0)
                throw ExceptionFactory.LifeCountersAreInvalid();

            if (counters.CyclesSinceLastEvent < 0)
                throw ExceptionFactory.LifeCountersAreInvalid();
        }
    }
}
