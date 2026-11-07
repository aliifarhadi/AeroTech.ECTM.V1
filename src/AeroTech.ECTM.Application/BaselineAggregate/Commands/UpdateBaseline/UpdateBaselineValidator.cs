using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.UpdateBaseline
{
    public abstract class UpdateBaselineValidator<TCommand> : BaselineDraftValidator<TCommand>
        where TCommand : IUpdateBaselineCommand
    {
        protected UpdateBaselineValidator()
        {
            RuleFor(command => command.BaselineId).GreaterThan(0);
            RuleFor(command => command.ExpectedDraftRevision).GreaterThan(0);
        }
    }
}
