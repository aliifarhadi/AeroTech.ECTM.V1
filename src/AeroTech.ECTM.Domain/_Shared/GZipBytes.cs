using System.IO.Compression;

namespace AeroTech.ECTM.Domain._Shared
{
    public static class GZipBytes
    {
        public static byte[] Compress(byte[] raw)
        {
            using var target = new MemoryStream();

            using (var gzip = new GZipStream(target, CompressionLevel.Optimal, leaveOpen: true))
                gzip.Write(raw);

            return target.ToArray();
        }

        public static byte[] Decompress(byte[] compressed)
        {
            using var source = new MemoryStream(compressed, writable: false);
            using var gzip = new GZipStream(source, CompressionMode.Decompress);
            using var target = new MemoryStream();

            gzip.CopyTo(target);

            return target.ToArray();
        }
    }
}
