namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries
{
    public interface IFlightSeriesQuery
    {
        long FlightId { get; }

        string Parameters { get; }

        string Sampling { get; }

        int? From { get; }

        int? To { get; }

        int Budget { get; }
    }
}
