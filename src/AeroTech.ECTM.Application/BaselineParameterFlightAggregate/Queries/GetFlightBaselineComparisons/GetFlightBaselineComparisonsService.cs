using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto;
using AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Arguments;
using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Queries.GetFlightBaselineComparisons
{
    public sealed class GetFlightBaselineComparisonsService : IGetFlightBaselineComparisonsService
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;
        private readonly IBaselineParameterFlightRepository _evaluations;
        private readonly BaselineEvaluationDtoLoader _evaluationDtos;

        public GetFlightBaselineComparisonsService(
            IFlightRepository flights,
            IFlightStabilityPointsRepository stabilityPoints,
            IBaselineParameterFlightRepository evaluations,
            BaselineEvaluationDtoLoader evaluationDtos)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
            _evaluations = evaluations;
            _evaluationDtos = evaluationDtos;
        }

        public async Task<BackofficeFlightBaselineComparisonsDto> ExecuteAsync(long flightId, EvaluationFilterArgs filter, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);

            var currentPoints = await _stabilityPoints.FindCurrentAsync(flightId, cancellationToken);
            var evaluations = await _evaluations.ListCurrentForFlightAsync(flightId, filter, cancellationToken);
            var evaluationDtos = await _evaluationDtos.LoadAsync(evaluations, cancellationToken);

            return new BackofficeFlightBaselineComparisonsDto(
                flight.Id,
                flight.FlightNumber,
                flight.FlightDate,
                EnumValueDto.Of(flight.State),
                currentPoints?.AnalysisRevisionId,
                evaluationDtos);
        }
    }
}
