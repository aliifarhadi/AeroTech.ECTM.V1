using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons
{
    public interface IGetFlightBaselineComparisonsService
    {
        Task<BackofficeFlightBaselineComparisonsDto> ExecuteAsync(long flightId, EvaluationFilterArgs filter, CancellationToken cancellationToken = default);
    }
}
