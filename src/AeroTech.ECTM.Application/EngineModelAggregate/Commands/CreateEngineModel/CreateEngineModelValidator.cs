using FluentValidation;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.CreateEngineModel
{
    public abstract class CreateEngineModelValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICreateEngineModelCommand
    {
        protected CreateEngineModelValidator()
        {
            RuleFor(command => command.Code).NotEmpty().MaximumLength(64);
            RuleFor(command => command.Manufacturer).NotEmpty().MaximumLength(128);
            RuleFor(command => command.Family).MaximumLength(64);
            RuleFor(command => command.VariantCode).MaximumLength(64);
            RuleFor(command => command.ThrustRatingCode).MaximumLength(64);
            RuleForEach(command => command.DocumentReferences).NotEmpty().MaximumLength(256);
            RuleFor(command => command.Notes).MaximumLength(2000);
        }
    }
}
