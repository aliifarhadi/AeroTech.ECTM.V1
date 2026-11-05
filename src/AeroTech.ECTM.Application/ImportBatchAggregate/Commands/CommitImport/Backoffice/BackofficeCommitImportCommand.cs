using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Commands.CommitImport.Backoffice
{
    public sealed record BackofficeCommitImportCommand(long BatchId, string IdempotencyKey) : IRequest<BackofficeImportBatchDto>, ICommitImportCommand;
}
