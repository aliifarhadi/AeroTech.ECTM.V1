using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Services
{
    public sealed class FlightQualityRecorder
    {
        private readonly IFlightQualityRepository _qualities;
        private readonly IIdGenerator _idGenerator;
        private readonly IClock _clock;

        public FlightQualityRecorder(IFlightQualityRepository qualities, IIdGenerator idGenerator, IClock clock)
        {
            _qualities = qualities;
            _idGenerator = idGenerator;
            _clock = clock;
        }

        public async Task<FlightQuality> RecordAsync(long flightId, ActiveQualityRules activeRules, QualityInput input, CancellationToken cancellationToken = default)
        {
            var current = await _qualities.GetCurrentAsync(flightId, cancellationToken);

            if (current is not null && current.QualityRulesDocumentId == activeRules.Document.Id)
                return current;

            current?.Supersede();

            var result = QualityEvaluator.Evaluate(activeRules.Rules, input);
            var quality = FlightQuality.Record(_idGenerator.NewId(), flightId, activeRules.Document.Id, activeRules.Document.VersionNumber, result, _clock.GetDateTime());

            await _qualities.AddAsync(quality, cancellationToken);

            return quality;
        }
    }
}
