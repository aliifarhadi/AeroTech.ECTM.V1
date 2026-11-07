using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Dto
{
    public sealed record BackofficeFlightBaselineComparisonsDto(
        long FlightId,
        string? FlightNumber,
        DateOnly? FlightDate,
        EnumValueDto FlightState,
        long? CurrentAnalysisRevisionId,
        IReadOnlyList<BackofficeBaselineEvaluationDto> Evaluations);
}
