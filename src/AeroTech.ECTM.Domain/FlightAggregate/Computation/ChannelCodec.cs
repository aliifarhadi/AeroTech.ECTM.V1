using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text.Json;
using AeroTech.ECTM.Domain._Shared;

namespace AeroTech.ECTM.Domain.FlightAggregate.Computation
{
    public static class ChannelCodec
    {
        private const int BytesPerNumber = sizeof(double);

        public static EncodedChannel EncodeNumbers(IReadOnlyList<double?> values)
        {
            var raw = new byte[values.Count * BytesPerNumber];

            for (var index = 0; index < values.Count; index++)
            {
                var value = values[index] ?? double.NaN;
                var target = raw.AsSpan(index * BytesPerNumber, BytesPerNumber);

                BinaryPrimitives.WriteDoubleLittleEndian(target, value);
            }

            return Encoded(raw);
        }

        public static double?[] DecodeNumbers(byte[] payload)
        {
            var raw = GZipBytes.Decompress(payload);
            var values = new double?[raw.Length / BytesPerNumber];

            for (var index = 0; index < values.Length; index++)
            {
                var source = raw.AsSpan(index * BytesPerNumber, BytesPerNumber);
                var value = BinaryPrimitives.ReadDoubleLittleEndian(source);

                values[index] = double.IsNaN(value) ? null : value;
            }

            return values;
        }

        public static EncodedChannel EncodeTexts(IReadOnlyList<string?> values)
        {
            var raw = JsonSerializer.SerializeToUtf8Bytes(values);

            return Encoded(raw);
        }

        public static string?[] DecodeTexts(byte[] payload)
        {
            var raw = GZipBytes.Decompress(payload);

            return JsonSerializer.Deserialize<string?[]>(raw)!;
        }

        private static EncodedChannel Encoded(byte[] raw)
        {
            var sha256 = Convert.ToHexStringLower(SHA256.HashData(raw));

            return new EncodedChannel(GZipBytes.Compress(raw), sha256);
        }
    }
}
