using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById;
using AeroTech.ECTM.Application.AircraftAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;
using AeroTech.Framework.Core.ServiceContracts;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft
{
    public sealed class CreateAircraftService : ICreateAircraftService
    {
        private readonly IAircraftRepository _aircraft;
        private readonly IAircraftLock _aircraftLock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator _idGenerator;
        private readonly IGetAircraftByIdService _getAircraftById;

        public CreateAircraftService(
            IAircraftRepository aircraft,
            IAircraftLock aircraftLock,
            IUnitOfWork unitOfWork,
            IIdGenerator idGenerator,
            IGetAircraftByIdService getAircraftById)
        {
            _aircraft = aircraft;
            _aircraftLock = aircraftLock;
            _unitOfWork = unitOfWork;
            _idGenerator = idGenerator;
            _getAircraftById = getAircraftById;
        }

        public async Task<BackofficeAircraftDto> CreateAsync(ICreateAircraftCommand command, CancellationToken cancellationToken = default)
        {
            var aircraft = Aircraft.Register(_idGenerator.NewId(), command.Registration, command.TypeCode, command.OperatorCode);

            await using (await _aircraftLock.AcquireRegistrationAsync(aircraft.Registration, cancellationToken))
            {
                if (await _aircraft.RegistrationExistsAsync(aircraft.Registration, cancellationToken))
                    throw ExceptionFactory.AircraftRegistrationAlreadyExists(aircraft.Registration);

                await _aircraft.AddAsync(aircraft, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return await _getAircraftById.ExecuteAsync(aircraft.Id, cancellationToken);
        }
    }
}
