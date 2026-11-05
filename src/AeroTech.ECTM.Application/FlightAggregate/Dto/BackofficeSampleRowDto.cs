namespace AeroTech.ECTM.Application.FlightAggregate.Dto
{
    public sealed record BackofficeSampleRowDto(int SampleIndex, IReadOnlyList<string> Cells);
}
