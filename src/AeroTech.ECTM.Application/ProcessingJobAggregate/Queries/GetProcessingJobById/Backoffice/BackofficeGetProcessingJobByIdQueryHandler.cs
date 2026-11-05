using AeroTech.ECTM.Application.ProcessingJobAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ProcessingJobAggregate.Queries.GetProcessingJobById.Backoffice
{
    public sealed class BackofficeGetProcessingJobByIdQueryHandler : IRequestHandler<BackofficeGetProcessingJobByIdQuery, BackofficeProcessingJobDto>
    {
        private readonly IGetProcessingJobByIdService _service;

        public BackofficeGetProcessingJobByIdQueryHandler(IGetProcessingJobByIdService service) => _service = service;

        public Task<BackofficeProcessingJobDto> Handle(BackofficeGetProcessingJobByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.JobId, cancellationToken);
    }
}
