using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft.Backoffice
{
    public sealed record BackofficeUpdateAircraftCommand(
        long AircraftId,
        string? TypeCode,
        string? OperatorCode,
        AircraftStatus Status) : IRequest<BackofficeAircraftDto>, IUpdateAircraftCommand;
}
