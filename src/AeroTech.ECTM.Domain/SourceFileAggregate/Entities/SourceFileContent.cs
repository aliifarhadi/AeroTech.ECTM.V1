using AeroTech.ECTM.Domain._Shared;
using AeroTech.Framework.Core.Domain.Entities;

namespace AeroTech.ECTM.Domain.SourceFileAggregate.Entities
{
    public sealed class SourceFileContent : Entity<long>, IBulkData
    {
        private SourceFileContent()
        {
        }

        internal SourceFileContent(long sourceFileId, byte[] bytes)
        {
            Id = sourceFileId;
            CompressedBytes = GZipBytes.Compress(bytes);
        }

        public byte[] CompressedBytes { get; private set; } = default!;

        public static byte[] Unpack(byte[] compressedBytes) => GZipBytes.Decompress(compressedBytes);
    }
}
