using AeroTech.ECTM.Application.EngineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineAggregate.Queries.GetEngineById.Backoffice
{
    public sealed record BackofficeGetEngineByIdQuery(long EngineId) : IRequest<BackofficeEngineDetailDto>;
}
