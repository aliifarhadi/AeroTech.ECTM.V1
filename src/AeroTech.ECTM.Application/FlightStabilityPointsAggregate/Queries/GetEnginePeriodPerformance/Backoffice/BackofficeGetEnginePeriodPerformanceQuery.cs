using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetEnginePeriodPerformance.Backoffice
{
    public sealed record BackofficeGetEnginePeriodPerformanceQuery(long EngineId, DateOnly? From, DateOnly? To, SnapshotKind Kind, string? Selector)
        : IRequest<BackofficePeriodPerformanceDto>;
}
