using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries
{
    public interface IGetFlightSeriesService
    {
        Task<BackofficeFlightSeriesDto> ExecuteAsync(IFlightSeriesQuery query, CancellationToken cancellationToken = default);
    }
}
