using AeroTech.ECTM.Application.AircraftAggregate.Dto;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft
{
    public interface ICreateAircraftService
    {
        Task<BackofficeAircraftDto> CreateAsync(ICreateAircraftCommand command, CancellationToken cancellationToken = default);
    }
}
