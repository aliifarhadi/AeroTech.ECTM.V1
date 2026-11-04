using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.Framework.Core.Domain.Repository;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft
{
    public sealed class UpdateAircraftService : IUpdateAircraftService
    {
        private readonly IAircraftRepository _aircraft;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGetAircraftByIdService _getAircraftById;

        public UpdateAircraftService(IAircraftRepository aircraft, IUnitOfWork unitOfWork, IGetAircraftByIdService getAircraftById)
        {
            _aircraft = aircraft;
            _unitOfWork = unitOfWork;
            _getAircraftById = getAircraftById;
        }

        public async Task<BackofficeAircraftDto> UpdateAsync(IUpdateAircraftCommand command, CancellationToken cancellationToken = default)
        {
            var aircraft = await _aircraft.GetAsync(command.AircraftId, cancellationToken)
                           ?? throw ExceptionFactory.AircraftNotFound(command.AircraftId);

            aircraft.Update(command.TypeCode, command.OperatorCode, command.Status);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return await _getAircraftById.ExecuteAsync(aircraft.Id, cancellationToken);
        }
    }
}
