namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto
{
    public sealed record BackofficeEngineRecomputeDto(long EngineId, int AcceptedFlights, int Queued, IReadOnlyList<BackofficeEngineRecomputeItemDto> Flights);
}
