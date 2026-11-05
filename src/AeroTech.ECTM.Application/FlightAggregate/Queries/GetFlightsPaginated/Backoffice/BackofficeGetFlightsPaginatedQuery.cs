using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application._Shared.Paging;
using AeroTech.Framework.Core.Domain.Queries;
using AeroTech.Messages.Ectm.Enums;
using MediatR;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightsPaginated.Backoffice
{
    public sealed class BackofficeGetFlightsPaginatedQuery : BackofficePageQuery, IRequest<PaginatedList<BackofficeFlightSummaryDto>>, IFlightsPaginatedQuery
    {
        public FlightState? State { get; set; }

        public long? AircraftId { get; set; }

        public long? EngineId { get; set; }

        public DateOnly? From { get; set; }

        public DateOnly? To { get; set; }

        public long? BatchId { get; set; }

        public IdentityStatus? IdentityStatus { get; set; }
    }
}
