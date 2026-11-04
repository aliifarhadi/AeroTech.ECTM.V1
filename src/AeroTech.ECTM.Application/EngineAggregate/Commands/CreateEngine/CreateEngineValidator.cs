using FluentValidation;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.CreateEngine
{
    public abstract class CreateEngineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateEngineCommand
    {
        protected CreateEngineValidator()
        {
            RuleFor(command => command.SerialNumber).NotEmpty().MaximumLength(64);
            RuleFor(command => command.EngineModelId).GreaterThan(0);
            RuleFor(command => command.Status).IsInEnum();
            RuleFor(command => command.VariantCode).MaximumLength(64);
            RuleFor(command => command.ThrustRatingCode).MaximumLength(64);
            RuleFor(command => command.ConfigurationEvidenceReference).MaximumLength(256);
            RuleFor(command => command.LifeCounters!)
                .SetValidator(new LifeCountersInputValidator())
                .When(command => command.LifeCounters is not null);
        }
    }
}
