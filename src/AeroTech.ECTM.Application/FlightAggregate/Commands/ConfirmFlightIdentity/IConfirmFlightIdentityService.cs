using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightAggregate.Commands.ConfirmFlightIdentity
{
    public interface IConfirmFlightIdentityService
    {
        Task<BackofficeFlightDetailDto> ConfirmAsync(IConfirmFlightIdentityCommand command, CancellationToken cancellationToken = default);
    }
}
