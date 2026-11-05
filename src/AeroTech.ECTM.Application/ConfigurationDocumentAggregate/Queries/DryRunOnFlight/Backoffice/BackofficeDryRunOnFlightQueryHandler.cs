using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight.Backoffice
{
    public sealed class BackofficeDryRunOnFlightQueryHandler : IRequestHandler<BackofficeDryRunOnFlightQuery, BackofficeFlightDryRunDto>
    {
        private readonly IDryRunOnFlightService _service;

        public BackofficeDryRunOnFlightQueryHandler(IDryRunOnFlightService service) => _service = service;

        public Task<BackofficeFlightDryRunDto> Handle(BackofficeDryRunOnFlightQuery query, CancellationToken cancellationToken)
            => _service.ExecuteAsync(query.Kind, query.DocumentId, query.FlightId, query.Budget, cancellationToken);
    }
}
