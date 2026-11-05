using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSeries
{
    public sealed class GetFlightSeriesService : IGetFlightSeriesService
    {
        public const int DefaultBudget = 2_000;
        public const int MaximumBudget = 20_000;
        public const int MaximumParameters = 16;

        private readonly IFlightRepository _flights;

        public GetFlightSeriesService(IFlightRepository flights) => _flights = flights;

        public static IReadOnlyList<string> KeysOf(string parameters)
            => parameters
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.Ordinal)
                .ToList();

        public async Task<BackofficeFlightSeriesDto> ExecuteAsync(IFlightSeriesQuery query, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.GetAsync(query.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(query.FlightId);

            var lastIndex = flight.SampleCount - 1;
            var from = Math.Min(query.From ?? 0, lastIndex);
            var to = Math.Min(query.To ?? lastIndex, lastIndex);
            var rangeLength = to - from + 1;

            if (query.Sampling == SeriesSampler.Raw && rangeLength > MaximumBudget)
                throw ExceptionFactory.SeriesRangeTooLarge(rangeLength, MaximumBudget);

            var timeChannel = ChannelOf(flight, flight.TimeParameterKey ?? string.Empty);
            var channels = KeysOf(query.Parameters).Select(key => ChannelOf(flight, key)).ToList();
            var payloadIds = channels.Append(timeChannel).Select(channel => channel.Id).Distinct().ToList();
            var payloads = await _flights.ListChannelPayloadsAsync(payloadIds, cancellationToken);
            var payloadById = payloads.ToDictionary(payload => payload.Id);
            var elapsed = timeChannel.NumbersFrom(payloadById[timeChannel.Id]);

            var series = channels
                .Select(channel => SeriesOf(channel, payloadById[channel.Id], elapsed, from, to, query))
                .ToList();

            return new BackofficeFlightSeriesDto(flight.Id, from, to, series);
        }

        private static BackofficeParameterSeriesDto SeriesOf(FlightChannel channel, FlightChannelPayload payload, IReadOnlyList<double?> elapsed, int from, int to, IFlightSeriesQuery query)
        {
            var values = channel.NumbersFrom(payload);
            var sampled = SeriesSampler.Sample(elapsed, values, from, to, query.Sampling, query.Budget);

            return new BackofficeParameterSeriesDto(channel.ParameterKey, channel.Unit, query.Sampling, sampled.SampleCountInRange, sampled.Points, sampled.Gaps);
        }

        private static FlightChannel ChannelOf(Flight flight, string parameterKey)
        {
            var channel = flight.Channels.FirstOrDefault(candidate => candidate.ParameterKey == parameterKey);

            if (channel is null || !channel.IsNumeric)
                throw ExceptionFactory.FlightParameterNotFound(flight.Id, parameterKey);

            return channel;
        }
    }
}
