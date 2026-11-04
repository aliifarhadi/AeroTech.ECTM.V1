using AeroTech.ECTM.Application.AircraftAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.AircraftAggregate.Queries.GetAircraftById.Backoffice
{
    public sealed class BackofficeGetAircraftByIdQueryHandler : IRequestHandler<BackofficeGetAircraftByIdQuery, BackofficeAircraftDto>
    {
        private readonly IGetAircraftByIdService _service;

        public BackofficeGetAircraftByIdQueryHandler(IGetAircraftByIdService service) => _service = service;

        public Task<BackofficeAircraftDto> Handle(BackofficeGetAircraftByIdQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.AircraftId, cancellationToken);
    }
}
