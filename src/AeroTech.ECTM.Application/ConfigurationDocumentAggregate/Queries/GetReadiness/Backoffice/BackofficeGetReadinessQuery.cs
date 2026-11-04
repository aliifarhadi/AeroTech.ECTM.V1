using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetReadiness.Backoffice
{
    public sealed record BackofficeGetReadinessQuery : IRequest<BackofficeReadinessDto>;
}
