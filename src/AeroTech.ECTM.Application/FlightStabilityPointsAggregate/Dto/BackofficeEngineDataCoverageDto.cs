using AeroTech.ECTM.Application._Shared.Enums;

namespace AeroTech.ECTM.Application.FlightStabilityPointsAggregate.Dto
{
    public sealed record BackofficeEngineDataCoverageDto(
        long EngineId,
        string SerialNumber,
        DateOnly? From,
        DateOnly? To,
        int FlightsReceived,
        IReadOnlyList<BackofficeStateCountDto> FlightStates,
        IReadOnlyList<BackofficeStateCountDto> Grades,
        IReadOnlyList<BackofficeAvailabilityCountDto> Availability,
        IReadOnlyList<BackofficeMissingChannelDto> MissingChannels);

    public sealed record BackofficeStateCountDto(EnumValueDto State, int Flights);

    public sealed record BackofficeAvailabilityCountDto(EnumValueDto Kind, string? Selector, int Available, IReadOnlyDictionary<string, int> NotEvaluableReasons);

    public sealed record BackofficeMissingChannelDto(string ParameterKey, int FlightsNotRecorded, int FlightsBelowFullCoverage);
}
