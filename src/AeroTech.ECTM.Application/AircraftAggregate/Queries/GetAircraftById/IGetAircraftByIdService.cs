using AeroTech.ECTM.Application.AircraftAggregate.Dto;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById
{
    public interface IGetAircraftByIdService
    {
        Task<BackofficeAircraftDto> ExecuteAsync(long aircraftId, CancellationToken cancellationToken = default);
    }
}
