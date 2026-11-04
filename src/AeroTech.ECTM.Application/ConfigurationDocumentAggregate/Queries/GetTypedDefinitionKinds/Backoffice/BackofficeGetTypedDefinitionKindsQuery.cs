using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetTypedDefinitionKinds.Backoffice
{
    public sealed record BackofficeGetTypedDefinitionKindsQuery : IRequest<IReadOnlyList<BackofficeTypedDefinitionKindDto>>;
}
