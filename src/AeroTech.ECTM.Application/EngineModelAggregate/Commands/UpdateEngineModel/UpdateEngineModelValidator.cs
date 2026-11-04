using FluentValidation;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel
{
    public abstract class UpdateEngineModelValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : IUpdateEngineModelCommand
    {
        protected UpdateEngineModelValidator()
        {
            RuleFor(command => command.Manufacturer).NotEmpty().MaximumLength(128);
            RuleFor(command => command.Family).MaximumLength(64);
            RuleFor(command => command.VariantCode).MaximumLength(64);
            RuleFor(command => command.ThrustRatingCode).MaximumLength(64);
            RuleForEach(command => command.DocumentReferences).NotEmpty().MaximumLength(256);
            RuleFor(command => command.Notes).MaximumLength(2000);
        }
    }
}
