using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetEngineBaselines.Backoffice
{
    public sealed record BackofficeGetEngineBaselinesQuery(long EngineId) : IRequest<BackofficeEngineBaselinesDto>;
}
