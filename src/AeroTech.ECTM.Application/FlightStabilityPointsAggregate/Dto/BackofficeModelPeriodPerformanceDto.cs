using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeModelPeriodPerformanceDto(
        long EngineModelId,
        DateOnly? From,
        DateOnly? To,
        EnumValueDto Kind,
        string? Selector,
        IReadOnlyList<BackofficePeriodPerformanceDto> Engines,
        IReadOnlyList<BackofficeParameterSpreadDto> Spread);

    public sealed record BackofficeParameterSpreadDto(string Parameter, int Engines, double? LowestEngineMean, double? HighestEngineMean, double? Spread);
}
