using System.Security.Cryptography;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain.SourceFileAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.SourceFileAggregate
{
    public sealed class SourceFile : AggregateRoot<long>
    {
        private SourceFile()
        {
        }

        private SourceFile(long id, string fileName, string sha256, long sizeBytes, SourceType sourceType, DateTimeOffset storedAt, byte[] bytes)
        {
            Id = id;
            FileName = fileName;
            Sha256 = sha256;
            SizeBytes = sizeBytes;
            SourceType = sourceType;
            StoredAt = storedAt;
            Content = new SourceFileContent(id, bytes);
        }

        public string FileName { get; private set; } = default!;

        public string Sha256 { get; private set; } = default!;

        public long SizeBytes { get; private set; }

        public SourceType SourceType { get; private set; }

        public DateTimeOffset StoredAt { get; private set; }

        public SourceFileContent? Content { get; private set; }

        public static SourceFile Store(long id, string fileName, byte[] bytes, DateTimeOffset now)
        {
            var bareName = SourceFileKinds.BareName(fileName);
            var sourceType = SourceTypeOf(bareName);

            return new SourceFile(id, bareName, Sha256Of(bytes), bytes.LongLength, sourceType, now, bytes);
        }

        public static string Sha256Of(byte[] bytes) => Convert.ToHexStringLower(SHA256.HashData(bytes));

        public static SourceType SourceTypeOf(string fileName)
        {
            if (SourceFileKinds.IsSynthetic(fileName))
                return SourceType.SYNTHETIC_TEST_ONLY;

            return SourceType.REAL_SOURCE;
        }
    }
}
