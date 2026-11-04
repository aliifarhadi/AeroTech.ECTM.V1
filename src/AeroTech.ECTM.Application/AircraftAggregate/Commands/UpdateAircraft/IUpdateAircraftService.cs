using AeroTech.ECTM.Application.AircraftAggregate.Dto;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft
{
    public interface IUpdateAircraftService
    {
        Task<BackofficeAircraftDto> UpdateAsync(IUpdateAircraftCommand command, CancellationToken cancellationToken = default);
    }
}
