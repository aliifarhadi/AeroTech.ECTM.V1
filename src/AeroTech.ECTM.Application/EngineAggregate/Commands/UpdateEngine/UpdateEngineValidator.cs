using FluentValidation;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands.UpdateEngine
{
    public abstract class UpdateEngineValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IUpdateEngineCommand
    {
        protected UpdateEngineValidator()
        {
            RuleFor(command => command.Status).IsInEnum();
            RuleFor(command => command.VariantCode).MaximumLength(64);
            RuleFor(command => command.ThrustRatingCode).MaximumLength(64);
            RuleFor(command => command.ConfigurationEvidenceReference).MaximumLength(256);
        }
    }
}
