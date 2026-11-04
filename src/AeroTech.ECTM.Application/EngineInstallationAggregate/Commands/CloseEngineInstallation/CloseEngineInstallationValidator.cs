using FluentValidation;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CloseEngineInstallation
{
    public abstract class CloseEngineInstallationValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICloseEngineInstallationCommand
    {
        protected CloseEngineInstallationValidator()
        {
            RuleFor(command => command.RemovedUtc).NotEmpty();
            RuleFor(command => command.Reason).NotEmpty().MaximumLength(256);
        }
    }
}
