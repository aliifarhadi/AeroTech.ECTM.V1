using FluentValidation;

namespace AeroTech.ECTM.Application.EngineAggregate.Commands
{
    public sealed class LifeCountersInputValidator : AbstractValidator<LifeCountersInput>
    {
        public LifeCountersInputValidator()
        {
            RuleFor(input => input.BasisUtc).NotEmpty();
        }
    }
}
