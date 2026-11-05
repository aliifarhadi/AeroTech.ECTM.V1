using AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Queries.GetModelPeriodPerformance.Backoffice
{
    public sealed record BackofficeGetModelPeriodPerformanceQuery(long EngineModelId, DateOnly? From, DateOnly? To, SnapshotKind Kind, string? Selector)
        : IRequest<BackofficeModelPeriodPerformanceDto>;
}
