using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight
{
    public interface IDryRunOnFlightService
    {
        Task<BackofficeFlightDryRunDto> ExecuteAsync(string kind, long documentId, long flightId, int budget, CancellationToken cancellationToken = default);
    }
}
