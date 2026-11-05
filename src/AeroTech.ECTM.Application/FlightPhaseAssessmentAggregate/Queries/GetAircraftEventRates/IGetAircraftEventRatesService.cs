using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Queries.GetAircraftEventRates
{
    public interface IGetAircraftEventRatesService
    {
        Task<BackofficeAircraftEventRatesDto> ExecuteAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default);
    }
}
