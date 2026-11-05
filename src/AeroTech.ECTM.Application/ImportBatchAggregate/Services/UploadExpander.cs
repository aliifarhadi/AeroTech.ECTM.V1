using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.Domain.Exceptions;
using Microsoft.Extensions.Options;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace AeroTech.ECTM.Application.ImportBatchAggregate.Services
{
    public sealed class UploadExpander
    {
        private const int CopyBufferSize = 81_920;

        private readonly ImportOptions _options;

        public UploadExpander(IOptions<ImportOptions> options) => _options = options.Value;

        public IReadOnlyList<SourceMember> Expand(IReadOnlyList<UploadedFile> files)
        {
            var members = new List<SourceMember>();
            var expandedBytes = 0L;

            foreach (var file in files)
            {
                var name = SourceFileKinds.BareName(file.Name);

                if (!SourceFileKinds.IsRecording(name) && !SourceFileKinds.IsArchive(name))
                    throw ExceptionFactory.ImportFileTypeNotSupported(name);

                if (SourceFileKinds.IsRecording(name))
                    EnsureFileWithinLimit(name, file.Content.LongLength);

                var fileMembers = SourceFileKinds.IsArchive(name)
                    ? ReadArchive(name, file.Content, _options.MaximumBatchBytes - expandedBytes)
                    : [new SourceMember(name, null, file.Content)];

                foreach (var member in fileMembers)
                {
                    expandedBytes += member.Content.LongLength;
                    members.Add(member);
                }

                if (members.Count > _options.MaximumBatchFiles)
                    throw ExceptionFactory.ImportHasTooManyFiles(members.Count, _options.MaximumBatchFiles);

                if (expandedBytes > _options.MaximumBatchBytes)
                    throw ExceptionFactory.ImportExpandsBeyondLimit(_options.MaximumBatchBytes);
            }

            return members;
        }

        private IReadOnlyList<SourceMember> ReadArchive(string archiveName, byte[] content, long remainingBytes)
        {
            try
            {
                using var stream = new MemoryStream(content, writable: false);
                using var archive = ArchiveFactory.Open(stream);

                if (archive.Type is not (ArchiveType.Zip or ArchiveType.Rar))
                    throw ExceptionFactory.ImportFileTypeNotSupported(archiveName);

                var entries = archive.Entries.Where(entry => !entry.IsDirectory).ToList();

                if (entries.Any(entry => entry.IsEncrypted))
                    throw ExceptionFactory.ArchiveIsEncrypted(archiveName);

                if (entries.Count == 0)
                    throw ExceptionFactory.ArchiveHasNoMembers(archiveName);

                if (entries.Count > _options.MaximumBatchFiles)
                    throw ExceptionFactory.ImportHasTooManyFiles(entries.Count, _options.MaximumBatchFiles);

                var members = new List<SourceMember>(entries.Count);
                var expanded = 0L;

                foreach (var entry in entries)
                {
                    var memberName = SourceFileKinds.BareName(entry.Key!);

                    using var source = entry.OpenEntryStream();

                    var memberContent = ReadBounded(source, remainingBytes - expanded);

                    expanded += memberContent.LongLength;
                    members.Add(new SourceMember(memberName, archiveName, memberContent));
                }

                return members;
            }
            catch (Exception exception) when (exception is not BusinessException)
            {
                throw ExceptionFactory.ArchiveIsUnreadable(archiveName);
            }
        }

        private byte[] ReadBounded(Stream source, long remainingBytes)
        {
            using var target = new MemoryStream();

            var buffer = new byte[CopyBufferSize];
            int read;

            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (target.Length + read > remainingBytes)
                    throw ExceptionFactory.ImportExpandsBeyondLimit(_options.MaximumBatchBytes);

                target.Write(buffer, 0, read);
            }

            return target.ToArray();
        }

        private void EnsureFileWithinLimit(string name, long sizeBytes)
        {
            if (sizeBytes > _options.MaximumFileBytes)
                throw ExceptionFactory.ImportFileIsTooLarge(name, sizeBytes, _options.MaximumFileBytes);
        }
    }
}
