using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Commands.EvaluateFlightQuality
{
    public sealed class EvaluateFlightQualityService : IEvaluateFlightQualityService
    {
        private readonly IFlightRepository _flights;
        private readonly ActiveQualityRulesLoader _activeRules;
        private readonly FlightQualityInputLoader _inputs;
        private readonly FlightQualityRecorder _recorder;
        private readonly IUnitOfWork _unitOfWork;

        public EvaluateFlightQualityService(
            IFlightRepository flights,
            ActiveQualityRulesLoader activeRules,
            FlightQualityInputLoader inputs,
            FlightQualityRecorder recorder,
            IUnitOfWork unitOfWork)
        {
            _flights = flights;
            _activeRules = activeRules;
            _inputs = inputs;
            _recorder = recorder;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeFlightQualityDto> EvaluateAsync(long flightId, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);
            var activeRules = await _activeRules.LoadAsync(cancellationToken);
            var input = await _inputs.LoadAsync(flight, activeRules.Rules, cancellationToken);
            var quality = await _recorder.RecordAsync(flight.Id, activeRules, input, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BackofficeFlightQualityDto.From(quality);
        }
    }
}
