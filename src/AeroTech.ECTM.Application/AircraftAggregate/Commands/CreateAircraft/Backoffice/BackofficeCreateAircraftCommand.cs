using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft.Backoffice
{
    public sealed record BackofficeCreateAircraftCommand(
        string Registration,
        string? TypeCode,
        string? OperatorCode) : IRequest<BackofficeAircraftDto>, ICreateAircraftCommand;
}
