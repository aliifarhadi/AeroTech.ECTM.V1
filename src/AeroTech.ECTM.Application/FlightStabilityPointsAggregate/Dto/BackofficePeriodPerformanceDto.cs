using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficePeriodPerformanceDto(
        long EngineId,
        string SerialNumber,
        DateOnly? From,
        DateOnly? To,
        EnumValueDto Kind,
        string? Selector,
        int Flights,
        int AcceptedFlights,
        int Contributors,
        IReadOnlyList<ValueDistribution> Parameters,
        IReadOnlyList<ValueDistribution> Conditions,
        IReadOnlyDictionary<string, int> Configurations);
}
