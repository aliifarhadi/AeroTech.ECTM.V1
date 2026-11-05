using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightQualityAggregate.Services
{
    public sealed class FlightQualityInputLoader
    {
        private readonly IFlightRepository _flights;

        public FlightQualityInputLoader(IFlightRepository flights) => _flights = flights;

        public async Task<QualityInput> LoadAsync(Flight flight, IReadOnlySet<string> neededCodes, CancellationToken cancellationToken = default)
        {
            var needed = flight.Channels
                .Where(channel => channel.ParameterKey == flight.TimeParameterKey || neededCodes.Contains(channel.ParameterCode))
                .ToList();
            var payloads = await _flights.ListChannelPayloadsAsync(needed.Select(channel => channel.Id).ToList(), cancellationToken);
            var payloadById = payloads.ToDictionary(payload => payload.Id);

            var numbers = needed
                .Where(channel => channel.IsNumeric)
                .ToDictionary(channel => channel.ParameterKey, channel => (IReadOnlyList<double?>)channel.NumbersFrom(payloadById[channel.Id]));
            var texts = needed
                .Where(channel => !channel.IsNumeric)
                .ToDictionary(channel => channel.ParameterKey, channel => (IReadOnlyList<string?>)channel.TextsFrom(payloadById[channel.Id]));
            var statistics = flight.Channels.Select(StatisticsOf).ToList();
            var elapsed = flight.TimeParameterKey is { } timeKey && numbers.TryGetValue(timeKey, out var time) ? time : [];

            return new QualityInput(flight.SampleCount, elapsed, numbers, texts, statistics, flight.BlockingCount > 0);
        }

        public static HashSet<string> CodesOf(QualityRulesContent rules)
        {
            var channels = rules.Channels!;
            var codes = new HashSet<string>(StringComparer.Ordinal)
            {
                channels.Altitude!,
                channels.RadioHeight!,
                channels.AirGround!,
                channels.Groundspeed!,
                channels.EngineN1!,
                channels.PrimaryClock!,
                channels.SecondaryClock!
            };

            foreach (var stuckChannel in rules.StuckSensor!.Channels!)
                codes.Add(stuckChannel.ParameterCode!);

            return codes;
        }

        private static ChannelStatistics StatisticsOf(FlightChannel channel)
        {
            var valued = channel.SampleCount - channel.MissingCount - channel.UnparseableCount;

            return new ChannelStatistics(channel.ParameterKey, channel.ParameterCode, channel.ProfileOrdinal, channel.SampleCount, valued, channel.OutOfRangeCount);
        }
    }
}
