using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.FlightQualityAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.AircraftAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Queries.GetAircraftQuality
{
    public sealed class GetAircraftQualityService : IGetAircraftQualityService
    {
        private const double FullCoverage = 100;

        private readonly IAircraftRepository _aircraft;
        private readonly IFlightRepository _flights;
        private readonly IFlightQualityRepository _qualities;

        public GetAircraftQualityService(IAircraftRepository aircraft, IFlightRepository flights, IFlightQualityRepository qualities)
        {
            _aircraft = aircraft;
            _flights = flights;
            _qualities = qualities;
        }

        public async Task<BackofficeAircraftQualityDto> ExecuteAsync(long aircraftId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken = default)
        {
            var aircraft = await _aircraft.GetAsync(aircraftId, cancellationToken)
                           ?? throw ExceptionFactory.AircraftNotFound(aircraftId);
            var flights = await _flights.ListForAircraftAsync(aircraft.Id, from, to, cancellationToken);
            var qualities = await _qualities.ListCurrentAsync(flights.Select(flight => flight.Id).ToList(), cancellationToken);
            var qualityByFlight = qualities.ToDictionary(quality => quality.FlightId);
            var results = qualities.Select(quality => quality.ReadResult()).ToList();

            return new BackofficeAircraftQualityDto(
                aircraft.Id,
                aircraft.Registration,
                from,
                to,
                flights.Count,
                qualities.Count,
                GradeCountsOf(qualities),
                qualities.Count(quality => quality.TimeTrust == TimeTrust.UNTRUSTED),
                results.Count(result => result.Clock?.DisagreeRows > 0),
                qualities.Count(quality => quality.DiscontinuityCount > 0),
                qualities.Count(quality => quality.PartialStart == true),
                qualities.Count(quality => quality.PartialEnd == true),
                ShortfallsOf(results),
                flights.Select(flight => RowOf(flight, qualityByFlight.GetValueOrDefault(flight.Id))).ToList());
        }

        private static List<BackofficeGradeCountDto> GradeCountsOf(IReadOnlyList<FlightQuality> qualities)
            => Enum.GetValues<QualityGrade>()
                .Select(grade => new BackofficeGradeCountDto(EnumValueDto.Of(grade), qualities.Count(quality => quality.Grade == grade)))
                .ToList();

        private static List<BackofficeChannelShortfallDto> ShortfallsOf(IReadOnlyList<QualityResult> results)
            => results
                .SelectMany(result => result.Coverage)
                .Where(channel => channel.Percent < FullCoverage)
                .GroupBy(channel => channel.ParameterKey)
                .Select(group => new BackofficeChannelShortfallDto(group.Key, group.Count(channel => !channel.Recorded), group.Count(channel => channel.Recorded)))
                .OrderBy(shortfall => shortfall.ParameterKey, StringComparer.Ordinal)
                .ToList();

        private static BackofficeFlightQualityRowDto RowOf(Flight flight, FlightQuality? quality)
            => new(
                flight.Id,
                flight.FlightDate,
                flight.FlightNumber,
                EnumValueDto.OfNullable(quality?.Grade),
                EnumValueDto.OfNullable(quality?.TimeTrust),
                quality?.DiscontinuityCount,
                quality?.PartialStart,
                quality?.PartialEnd);
    }
}
