using AeroTech.ECTM.Application.FlightReviewAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.FlightReviewAggregate.Commands.BulkReviewFlights.Backoffice
{
    public sealed class BackofficeBulkReviewFlightsCommandHandler : IRequestHandler<BackofficeBulkReviewFlightsCommand, BackofficeBulkReviewDto>
    {
        private readonly IBulkReviewFlightsService _service;

        public BackofficeBulkReviewFlightsCommandHandler(IBulkReviewFlightsService service) => _service = service;

        public Task<BackofficeBulkReviewDto> Handle(BackofficeBulkReviewFlightsCommand command, CancellationToken cancellationToken)
            => _service.ReviewAsync(command, cancellationToken);
    }
}
