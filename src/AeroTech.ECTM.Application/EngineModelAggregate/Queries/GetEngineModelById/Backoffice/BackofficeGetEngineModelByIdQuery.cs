using AeroTech.ECTM.Application.EngineModelAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Queries.GetEngineModelById.Backoffice
{
    public sealed record BackofficeGetEngineModelByIdQuery(long EngineModelId) : IRequest<BackofficeEngineModelDto>;
}
