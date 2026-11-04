namespace AeroTech.ECTM.Application.MaintenanceEventAggregate.Queries.GetMaintenanceEventsPaginated
{
    public interface IMaintenanceEventsPaginatedQuery
    {
        long? EngineId { get; }

        long? AircraftId { get; }

        int? Position { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
