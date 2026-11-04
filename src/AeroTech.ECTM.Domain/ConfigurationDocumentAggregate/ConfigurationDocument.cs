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

        private ConfigurationDocument(long id, ConfigurationKind kind, int versionNumber, long? basedOnDocumentId)
        {
            Id = id;
            Kind = kind;
            VersionNumber = versionNumber;
            Status = ConfigurationDocumentStatus.DRAFT;
            BasedOnDocumentId = basedOnDocumentId;
        }

        public ConfigurationKind Kind { get; private set; }

        public int VersionNumber { get; private set; }

        public ConfigurationDocumentStatus Status { get; private set; }

        public long? BasedOnDocumentId { get; private set; }

        public string Content { get; private set; } = default!;

        public string ContentSha256 { get; private set; } = default!;

        public DateTimeOffset? PublishedAt { get; private set; }

        public string? PublishedBy { get; private set; }

        public string? ApprovalNote { get; private set; }

        public DateTimeOffset? RetiredAt { get; private set; }

        public string? RetiredBy { get; private set; }

        public string? RetirementNote { get; private set; }

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

            var document = new ConfigurationDocument(id, kind, versionNumber, basedOnDocumentId);
            document.ReplaceContent(content);

            return document;
        }

        public void UpdateDraftContent(string content)
        {
            if (Status != ConfigurationDocumentStatus.DRAFT)
                throw ExceptionFactory.ConfigurationDocumentIsNotDraft(Id, Status);

            ReplaceContent(content);
        }

        public void Publish(string approvalNote, string? actor, DateTimeOffset now)
        {
            if (Status != ConfigurationDocumentStatus.DRAFT)
                throw ExceptionFactory.ConfigurationDocumentIsNotDraft(Id, Status);

            if (string.IsNullOrWhiteSpace(approvalNote))
                throw ExceptionFactory.ApprovalNoteIsRequired();

            Status = ConfigurationDocumentStatus.PUBLISHED;
            ApprovalNote = approvalNote.Trim();
            PublishedAt = now;
            PublishedBy = actor;
        }

        public void Retire(string? note, string? actor, DateTimeOffset now)
        {
            if (Status != ConfigurationDocumentStatus.PUBLISHED)
                throw ExceptionFactory.ConfigurationDocumentIsNotPublished(Id, Status);

            Status = ConfigurationDocumentStatus.RETIRED;
            RetirementNote = note?.Trim();
            RetiredAt = now;
            RetiredBy = actor;
        }

        private void ReplaceContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw ExceptionFactory.ConfigurationDocumentContentIsRequired();

            if (!IsJsonObject(content))
                throw ExceptionFactory.ConfigurationDocumentContentIsNotJsonObject();

            Content = content;
            ContentSha256 = Sha256Of(content);
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
