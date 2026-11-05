using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using Xunit;

namespace AeroTech.ECTM.Domain.ConformanceTests.Import;

public sealed class ChannelCodecTests
{
    [Fact]
    public void Numbers_round_trip_exactly_and_null_survives()
    {
        double?[] values = [0, -0.0, 97.85, null, double.MaxValue, double.Epsilon, -3.19, null];

        var encoded = ChannelCodec.EncodeNumbers(values);
        var decoded = ChannelCodec.DecodeNumbers(encoded.Payload);

        Assert.Equal(values.Length, decoded.Length);

        for (var index = 0; index < values.Length; index++)
            Assert.Equal(BitsOf(values[index]), BitsOf(decoded[index]));
    }

    [Fact]
    public void Texts_round_trip_exactly_and_null_survives()
    {
        string?[] values = ["GND", " AIR", null, "No Shake", "", "Ünïcode"];

        var decoded = ChannelCodec.DecodeTexts(ChannelCodec.EncodeTexts(values).Payload);

        Assert.Equal(values, decoded);
    }

    [Fact]
    public void The_checksum_depends_on_the_values_only()
    {
        double?[] values = [1, 2, null];

        Assert.Equal(ChannelCodec.EncodeNumbers(values).Sha256, ChannelCodec.EncodeNumbers(values).Sha256);
        Assert.NotEqual(ChannelCodec.EncodeNumbers(values).Sha256, ChannelCodec.EncodeNumbers([1, 2, 3]).Sha256);
    }

    private static long? BitsOf(double? value) => value is { } number ? BitConverter.DoubleToInt64Bits(number) : null;
}
