using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.SetPrimaryBaseline
{
    public abstract class SetPrimaryBaselineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ISetPrimaryBaselineCommand
    {
        protected SetPrimaryBaselineValidator()
        {
            RuleFor(command => command.EngineId).GreaterThan(0);
            RuleFor(command => command.BaselineParameterId).GreaterThan(0).When(command => command.BaselineParameterId is not null);
            RuleFor(command => command.SnapshotKind).NotNull().IsInEnum().When(command => command.BaselineParameterId is null)
                .WithMessage("To clear a primary, name its snapshotKind, selector and parameterCode.");
            RuleFor(command => command.ParameterCode).NotEmpty().MaximumLength(64).When(command => command.BaselineParameterId is null)
                .WithMessage("To clear a primary, name its snapshotKind, selector and parameterCode.");
            RuleFor(command => command.Selector).MaximumLength(64);
        }
    }
}
