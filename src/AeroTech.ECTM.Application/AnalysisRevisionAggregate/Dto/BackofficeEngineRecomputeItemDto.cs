using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.AnalysisRevisionAggregate.Dto
{
    public sealed record BackofficeEngineRecomputeItemDto(long FlightId, EnumValueDto Outcome, BackofficeAnalysisRevisionDto? Revision, long? ProcessingJobId);
}
