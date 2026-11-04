using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetConfigurationKinds.Backoffice
{
    public sealed record BackofficeGetConfigurationKindsQuery : IRequest<IReadOnlyList<BackofficeConfigurationKindDto>>;
}
