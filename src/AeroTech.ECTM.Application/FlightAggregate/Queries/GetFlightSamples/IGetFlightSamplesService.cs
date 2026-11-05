using AeroTech.ECTM.Application.FlightAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples
{
    public interface IGetFlightSamplesService
    {
        Task<BackofficeFlightSamplesDto> ExecuteAsync(IFlightSamplesQuery query, CancellationToken cancellationToken = default);
    }
}
