using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using MediatR;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons.Backoffice
{
    public sealed record BackofficeGetFlightBaselineComparisonsQuery(long FlightId, EvaluationFilterArgs Filter) : IRequest<BackofficeFlightBaselineComparisonsDto>;
}
