using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ConfigurationDocumentAggregate
{
    public sealed class ConfigurationDocument : AggregateRoot<long>
    {
        private ConfigurationDocument()
        {
        }

        private ConfigurationDocument(
            long id,
            ConfigurationKind kind,
            int versionNumber,
            long? basedOnDocumentId,
            string content,
            string contentSha256)
        {
            Id = id;
            Kind = kind;
            VersionNumber = versionNumber;
            Status = ConfigurationDocumentStatus.DRAFT;
            BasedOnDocumentId = basedOnDocumentId;
            Content = content;
            ContentSha256 = contentSha256;
        }

        public ConfigurationKind Kind { get; private set; }

        public int VersionNumber { get; private set; }

        public ConfigurationDocumentStatus Status { get; private set; }

        public long? BasedOnDocumentId { get; private set; }

        public string Content { get; private set; } = default!;

        public string ContentSha256 { get; private set; } = default!;

        public static ConfigurationDocument CreateDraft(
            long id,
            ConfigurationKind kind,
            int versionNumber,
            long? basedOnDocumentId,
            string content)
        {
            if (!Enum.IsDefined(kind))
                throw ExceptionFactory.ConfigurationKindNotFound(kind);

            if (versionNumber < 1)
                throw ExceptionFactory.ConfigurationDocumentVersionIsInvalid(versionNumber);

            if (string.IsNullOrWhiteSpace(content))
                throw ExceptionFactory.ConfigurationDocumentContentIsRequired();

            if (!IsJsonObject(content))
                throw ExceptionFactory.ConfigurationDocumentContentIsNotJsonObject();

            return new ConfigurationDocument(id, kind, versionNumber, basedOnDocumentId, content, Sha256Of(content));
        }

        private static bool IsJsonObject(string content)
        {
            try
            {
                using var document = JsonDocument.Parse(content);

                return document.RootElement.ValueKind == JsonValueKind.Object;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private static string Sha256Of(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var hash = SHA256.HashData(bytes);

            return Convert.ToHexStringLower(hash);
        }
    }
}
