using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById.Backoffice
{
    public sealed record BackofficeGetProcessingJobByIdQuery(long JobId) : IRequest<BackofficeProcessingJobDto>;
}
