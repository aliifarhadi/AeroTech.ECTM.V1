using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using MediatR;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight.Backoffice
{
    public sealed record BackofficeDryRunOnFlightQuery(
        string Kind,
        long DocumentId,
        long FlightId,
        int Budget) : IRequest<BackofficeFlightDryRunDto>;
}
