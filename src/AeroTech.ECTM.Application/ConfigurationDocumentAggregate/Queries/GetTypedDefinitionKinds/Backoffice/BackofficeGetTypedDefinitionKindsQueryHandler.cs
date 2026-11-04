using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.GetTypedDefinitionKinds.Backoffice
{
    public sealed class BackofficeGetTypedDefinitionKindsQueryHandler : IRequestHandler<BackofficeGetTypedDefinitionKindsQuery, IReadOnlyList<BackofficeTypedDefinitionKindDto>>
    {
        public Task<IReadOnlyList<BackofficeTypedDefinitionKindDto>> Handle(BackofficeGetTypedDefinitionKindsQuery query, CancellationToken cancellationToken)
        {
            IReadOnlyList<BackofficeTypedDefinitionKindDto> kinds = DerivedParameterKinds.All
                .Select(BackofficeTypedDefinitionKindDto.From)
                .ToList();

            return Task.FromResult(kinds);
        }
    }
}
