using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.UpdateAircraft
{
    public interface IUpdateAircraftCommand
    {
        long AircraftId { get; }

        string? TypeCode { get; }

        string? OperatorCode { get; }

        AircraftStatus Status { get; }
    }
}
