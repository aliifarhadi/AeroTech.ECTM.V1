using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.FlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity
{
    public sealed class ConfirmFlightIdentityService : IConfirmFlightIdentityService
    {
        private readonly IFlightRepository _flights;
        private readonly IAircraftRepository _aircraft;
        private readonly FlightIdentityService _identities;
        private readonly FlightDtoLoader _flightDtos;
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly AnalysisRulesLoader _rules;
        private readonly RecomputeScheduler _scheduler;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmFlightIdentityService(
            IFlightRepository flights,
            IAircraftRepository aircraft,
            FlightIdentityService identities,
            FlightDtoLoader flightDtos,
            IAnalysisRevisionRepository revisions,
            AnalysisRulesLoader rules,
            RecomputeScheduler scheduler,
            IUnitOfWork unitOfWork)
        {
            _flights = flights;
            _aircraft = aircraft;
            _identities = identities;
            _flightDtos = flightDtos;
            _revisions = revisions;
            _rules = rules;
            _scheduler = scheduler;
            _unitOfWork = unitOfWork;
        }

        public async Task<BackofficeFlightDetailDto> ConfirmAsync(IConfirmFlightIdentityCommand command, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(command.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(command.FlightId);
            var aircraft = await _aircraft.GetAsync(command.AircraftId, cancellationToken)
                           ?? throw ExceptionFactory.AircraftNotFound(command.AircraftId);
            var installations = await _identities.InstallationsOfAsync(aircraft.Id, cancellationToken);

            var identity = IdentityResolver.ConfirmByUser(aircraft, command.FlightDate, command.FlightNumber, command.Engines, flight.Positions, installations);
            var sameIdentity = await _flights.FindSameIdentityAsync(flight.Id, aircraft.Id, command.FlightDate, identity.FlightNumber!, cancellationToken);

            if (sameIdentity is not null && !command.ConfirmDistinctFlight)
                throw ExceptionFactory.PossibleDuplicateNeedsConfirmation(sameIdentity.Id);

            flight.ConfirmIdentity(identity, sameIdentity is not null, command.Note);

            await ScheduleRevisionForNewEnginesAsync(flight, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _flightDtos.LoadDetailAsync(flight, cancellationToken);
        }

        private async Task ScheduleRevisionForNewEnginesAsync(Flight flight, CancellationToken cancellationToken)
        {
            var current = await _revisions.FindCurrentAsync(flight.Id, cancellationToken);

            if (current is null || current.EngineAssignments == flight.EngineAssignmentKey())
                return;

            var rules = await _rules.LoadOfRevisionAsync(current, cancellationToken);

            await _scheduler.ScheduleAsync(flight, rules, RevisionTrigger.IDENTITY_CHANGE, cancellationToken);
        }
    }
}
