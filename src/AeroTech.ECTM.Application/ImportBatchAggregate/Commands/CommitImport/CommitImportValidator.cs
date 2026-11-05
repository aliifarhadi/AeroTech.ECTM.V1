using FluentValidation;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport
{
    public abstract class CommitImportValidator<TCommand> : AbstractValidator<TCommand>
        where TCommand : ICommitImportCommand
    {
        protected CommitImportValidator()
        {
            RuleFor(command => command.BatchId).GreaterThan(0);
            RuleFor(command => command.IdempotencyKey).NotEmpty().MaximumLength(128);
        }
    }
}
