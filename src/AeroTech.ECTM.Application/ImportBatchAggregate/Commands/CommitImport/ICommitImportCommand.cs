namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport
{
    public interface ICommitImportCommand
    {
        long BatchId { get; }

        string IdempotencyKey { get; }
    }
}
