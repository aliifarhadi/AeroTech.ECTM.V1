using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.GetBaselineById.Backoffice
{
    public sealed record BackofficeGetBaselineByIdQuery(long BaselineId) : IRequest<BackofficeBaselineDto>;
}
