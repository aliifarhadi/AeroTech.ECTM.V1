using AeroTech.ECTM.Application.BaselineAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineAggregate.Queries.PreviewBaseline.Backoffice
{
    public sealed record BackofficePreviewBaselineQuery(long BaselineId, long? FlightId) : IRequest<BackofficeBaselinePreviewDto>;
}
