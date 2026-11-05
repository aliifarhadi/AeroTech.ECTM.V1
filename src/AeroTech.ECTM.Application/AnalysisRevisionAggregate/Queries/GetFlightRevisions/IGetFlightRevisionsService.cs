using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Queries.GetFlightRevisions
{
    public interface IGetFlightRevisionsService
    {
        Task<IReadOnlyList<BackofficeAnalysisRevisionDto>> ExecuteAsync(long flightId, CancellationToken cancellationToken = default);
    }
}
