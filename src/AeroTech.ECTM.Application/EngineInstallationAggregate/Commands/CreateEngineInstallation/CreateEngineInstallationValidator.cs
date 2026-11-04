using FluentValidation;

namespace AeroTech.ECTM.Application.EngineInstallationAggregate.Commands.CreateEngineInstallation
{
    public abstract class CreateEngineInstallationValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateEngineInstallationCommand
    {
        protected CreateEngineInstallationValidator()
        {
            RuleFor(command => command.EngineId).GreaterThan(0);
            RuleFor(command => command.AircraftId).GreaterThan(0);
            RuleFor(command => command.InstalledUtc).NotEmpty();
            RuleFor(command => command.Reason).NotEmpty().MaximumLength(256);
            RuleFor(command => command.Evidence).MaximumLength(256);
        }
    }
}
