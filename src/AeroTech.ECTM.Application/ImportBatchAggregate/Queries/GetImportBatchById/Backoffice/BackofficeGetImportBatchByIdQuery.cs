using AeroTech.ECTM.Application.ImportBatchAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Queries.GetImportBatchById.Backoffice
{
    public sealed record BackofficeGetImportBatchByIdQuery(long BatchId) : IRequest<BackofficeImportBatchDto>;
}
