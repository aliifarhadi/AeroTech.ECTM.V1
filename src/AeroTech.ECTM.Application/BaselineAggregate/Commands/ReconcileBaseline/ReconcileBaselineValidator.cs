using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.ReconcileBaseline
{
    public abstract class ReconcileBaselineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IReconcileBaselineCommand
    {
        protected ReconcileBaselineValidator()
        {
            RuleFor(command => command.BaselineId).GreaterThan(0);
        }
    }
}
