using FluentValidation;

namespace AeroTech.ECTM.Application.BaselineAggregate.Commands.PublishBaseline
{
    public abstract class PublishBaselineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IPublishBaselineCommand
    {
        protected PublishBaselineValidator()
        {
            RuleFor(command => command.BaselineId).GreaterThan(0);
            RuleFor(command => command.ExpectedDraftRevision).GreaterThan(0);
            RuleFor(command => command.ConfirmedRawReference).Equal(true)
                .WithMessage("Confirm that the published values are raw references chosen by you: no environmental correction or OEM validation has been applied.");
        }
    }
}
