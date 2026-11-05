using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated
{
    public interface IFlightsPaginatedQuery
    {
        FlightState? State { get; }

        long? AircraftId { get; }

        long? EngineId { get; }

        DateOnly? From { get; }

        DateOnly? To { get; }

        long? BatchId { get; }

        IdentityStatus? IdentityStatus { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
