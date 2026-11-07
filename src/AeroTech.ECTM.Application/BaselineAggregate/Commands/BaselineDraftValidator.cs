using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands
{
    public abstract class BaselineDraftValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IBaselineDraftInput
    {
        public const int MaximumParameters = 60;

        protected BaselineDraftValidator()
        {
            RuleFor(command => command.Name).NotEmpty().MaximumLength(200);
            RuleFor(command => command.ShopVisitEventId).GreaterThan(0).When(command => command.ShopVisitEventId is not null);
            RuleFor(command => command.SourceFileName).MaximumLength(260);
            RuleFor(command => command.SourceFileSha256).Length(64).Matches("^[0-9a-fA-F]{64}$").When(command => command.SourceFileSha256 is not null);
            RuleFor(command => command.Notes).MaximumLength(2000);
            RuleFor(command => command.Parameters).NotNull();
            RuleFor(command => command.Parameters.Count).LessThanOrEqualTo(MaximumParameters).When(command => command.Parameters is not null);
            RuleForEach(command => command.Parameters).ChildRules(parameter =>
            {
                parameter.RuleFor(item => item.SnapshotKind).IsInEnum();
                parameter.RuleFor(item => item.Selector).MaximumLength(64);
                parameter.RuleFor(item => item.ParameterCode).NotEmpty().MaximumLength(64);
                parameter.RuleFor(item => item.SourceEnginePosition).InclusiveBetween(1, 2).When(item => item.SourceEnginePosition is not null);
                parameter.RuleFor(item => item.ReferenceConfiguration).MaximumLength(200);
                parameter.RuleFor(item => item.Notes).MaximumLength(2000);
                parameter.RuleForEach(item => item.ReferenceConditions).ChildRules(condition =>
                {
                    condition.RuleFor(value => value.ParameterCode).NotEmpty().MaximumLength(64);
                });
            });
        }
    }
}
