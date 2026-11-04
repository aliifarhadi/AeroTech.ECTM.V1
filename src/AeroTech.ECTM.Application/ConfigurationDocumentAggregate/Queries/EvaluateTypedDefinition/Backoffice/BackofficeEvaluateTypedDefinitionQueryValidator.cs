using System.Text.Json;
using FluentValidation;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.EvaluateTypedDefinition.Backoffice
{
    public sealed class BackofficeEvaluateTypedDefinitionQueryValidator : AbstractValidator<BackofficeEvaluateTypedDefinitionQuery>
    {
        public BackofficeEvaluateTypedDefinitionQueryValidator()
        {
            RuleFor(query => query.Definition.ValueKind)
                .Equal(JsonValueKind.Object)
                .WithMessage("The definition must be one JSON object.");
            RuleFor(query => query.Inputs).NotNull();
            RuleFor(query => query.Position).InclusiveBetween(1, 2).When(query => query.Position is not null);
        }
    }
}
