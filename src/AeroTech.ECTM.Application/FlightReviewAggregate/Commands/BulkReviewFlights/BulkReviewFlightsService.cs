using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using AeroTech.ECTM.Application.FlightReviewAggregate.Services;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightReviewAggregate.Computation;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.Domain.Exceptions;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights
{
    public sealed class BulkReviewFlightsService : IBulkReviewFlightsService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightQualityRepository _qualities;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;
        private readonly FlightReviewer _reviewer;
        private readonly IUnitOfWork _unitOfWork;

        public BulkReviewFlightsService(
            IFlightRepository flights,
            IFlightQualityRepository qualities,
            IFlightStabilityPointsRepository stabilityPoints,
            FlightReviewer reviewer,
            IUnitOfWork unitOfWork)
        {
            _flights = flights;
            _qualities = qualities;
            _stabilityPoints = stabilityPoints;
            _reviewer = reviewer;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeBulkReviewDto> ReviewAsync(IBulkReviewFlightsCommand command, CancellationToken cancellationToken = default)
        {
            var items = new List<BackofficeBulkReviewItemDto>();

            foreach (var flightId in command.FlightIds.Distinct())
                items.Add(await ReviewOneAsync(flightId, command, cancellationToken));

            var applied = items.Count(item => item.Outcome.Name == nameof(BulkReviewOutcome.APPLIED));

            return new BackofficeBulkReviewDto(EnumValueDto.Of(command.Action), applied, items.Count - applied, items);
        }

        private async Task<BackofficeBulkReviewItemDto> ReviewOneAsync(long flightId, IBulkReviewFlightsCommand command, CancellationToken cancellationToken)
        {
            var flight = await _flights.GetAsync(flightId, cancellationToken);

            if (flight is null)
                return Item(flightId, BulkReviewOutcome.NOT_FOUND, null, null, []);

            if (command.Action == ReviewAction.ACCEPT)
            {
                var refusals = await AcceptanceRefusalsAsync(flight, cancellationToken);

                if (refusals.Count > 0)
                    return Item(flight.Id, BulkReviewOutcome.REFUSED, flight.State, null, refusals);
            }

            try
            {
                await _reviewer.ApplyAsync(flight, new ReviewDecision(command.Action, command.ReasonCode, command.Note, null), cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Item(flight.Id, BulkReviewOutcome.APPLIED, flight.State, null, []);
            }
            catch (BusinessException exception)
            {
                return Item(flight.Id, BulkReviewOutcome.REFUSED, flight.State, exception.Code, []);
            }
        }

        private async Task<IReadOnlyList<AcceptanceRefusal>> AcceptanceRefusalsAsync(Flight flight, CancellationToken cancellationToken)
        {
            var quality = await _qualities.FindCurrentAsync(flight.Id, cancellationToken);
            var points = await _stabilityPoints.FindCurrentAsync(flight.Id, cancellationToken);

            return BulkAcceptance.RefusalsOf(flight.State, flight.IdentityStatus, quality?.Grade, flight.BlockingCount, points?.ReadResult());
        }

        private static BackofficeBulkReviewItemDto Item(long flightId, BulkReviewOutcome outcome, FlightState? state, int? errorCode, IReadOnlyList<AcceptanceRefusal> refusals)
            => new(flightId, EnumValueDto.Of(outcome), EnumValueDto.OfNullable(state), errorCode, refusals.Select(BackofficeAcceptanceRefusalDto.From).ToList());
    }
}
