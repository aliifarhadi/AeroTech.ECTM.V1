namespace AeroTech.ECTM.Application.AircraftAggregate.Commands.CreateAircraft
{
    public interface ICreateAircraftCommand
    {
        string Registration { get; }

        string? TypeCode { get; }

        string? OperatorCode { get; }
    }
}
