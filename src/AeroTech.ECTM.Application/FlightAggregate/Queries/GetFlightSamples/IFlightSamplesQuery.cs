namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples
{
    public interface IFlightSamplesQuery
    {
        long FlightId { get; }

        int? From { get; }

        int? To { get; }
    }
}
