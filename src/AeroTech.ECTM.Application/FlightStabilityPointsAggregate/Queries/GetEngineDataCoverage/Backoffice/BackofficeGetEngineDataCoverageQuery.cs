using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEngineDataCoverage.Backoffice
{
    public sealed record BackofficeGetEngineDataCoverageQuery(long EngineId, DateOnly? From, DateOnly? To) : IRequest<BackofficeEngineDataCoverageDto>;
}
