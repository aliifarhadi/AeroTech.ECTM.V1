namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Queries.GetEngineInstallationsPaginated
{
    public interface IEngineInstallationsPaginatedQuery
    {
        long? EngineId { get; }

        long? AircraftId { get; }

        int? Position { get; }

        bool OpenOnly { get; }

        int PageNumber { get; }

        int PageSize { get; }
    }
}
