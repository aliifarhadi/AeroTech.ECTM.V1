using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Arguments;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate
{
    public sealed class ImportBatch : AggregateRoot<long>
    {
        private readonly List<ImportItem> _items = new();

        private ImportBatch()
        {
        }

        private ImportBatch(long id, ConfigurationVersions versions, string? defaultRegistration, int? defaultYear, DateTimeOffset createdAt)
        {
            Id = id;
            State = ImportBatchState.PREVIEWED;
            DataProfileDocumentId = versions.DataProfileDocumentId;
            DataProfileVersion = versions.DataProfileVersion;
            ParameterCatalogDocumentId = versions.ParameterCatalogDocumentId;
            ParameterCatalogVersion = versions.ParameterCatalogVersion;
            DefaultRegistration = defaultRegistration;
            DefaultYear = defaultYear;
            CreatedAt = createdAt;
        }

        public ImportBatchState State { get; private set; }

        public long DataProfileDocumentId { get; private set; }

        public int DataProfileVersion { get; private set; }

        public long ParameterCatalogDocumentId { get; private set; }

        public int ParameterCatalogVersion { get; private set; }

        public string? DefaultRegistration { get; private set; }

        public int? DefaultYear { get; private set; }

        public string? IdempotencyKey { get; private set; }

        public long? JobId { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public DateTimeOffset? CommittedAt { get; private set; }

        public IReadOnlyCollection<ImportItem> Items => _items.AsReadOnly();

        public ConfigurationVersions Versions => new(DataProfileDocumentId, DataProfileVersion, ParameterCatalogDocumentId, ParameterCatalogVersion);

        public static ImportBatch Open(long id, ConfigurationVersions versions, string? defaultRegistration, int? defaultYear, DateTimeOffset now)
        {
            var registration = OptionalText.Normalize(defaultRegistration)?.ToUpperInvariant();

            return new ImportBatch(id, versions, registration, defaultYear, now);
        }

        public ImportItem AddItem(long itemId, ImportItemArgs item)
        {
            var importItem = new ImportItem(itemId, Id, _items.Count + 1, item);

            _items.Add(importItem);

            return importItem;
        }

        public bool StartCommit(string idempotencyKey, long jobId)
        {
            var started = State is ImportBatchState.COMMITTING or ImportBatchState.COMMITTED;

            if (started && IdempotencyKey == idempotencyKey)
                return false;

            if (started)
                throw ExceptionFactory.ImportBatchCommittedWithAnotherKey(Id, State);

            IdempotencyKey = idempotencyKey;
            JobId = jobId;
            State = ImportBatchState.COMMITTING;

            return true;
        }

        public IReadOnlyList<long> PendingItemIds()
            => _items
                .Where(item => item.IsPending)
                .OrderBy(item => item.Ordinal)
                .Select(item => item.Id)
                .ToList();

        public ImportItem ItemOf(long itemId) => _items.Single(item => item.Id == itemId);

        public void CommitItem(long itemId, long flightId) => ItemOf(itemId).Commit(flightId);

        public void LinkDuplicateItem(long itemId, long flightId) => ItemOf(itemId).LinkDuplicate(flightId);

        public void SkipItem(long itemId) => ItemOf(itemId).Skip();

        public void FailItem(long itemId, string errorCode) => ItemOf(itemId).Fail(errorCode);

        public void FinishCommit(DateTimeOffset now)
        {
            State = ImportBatchState.COMMITTED;
            CommittedAt = now;
        }

        public void FailCommit()
        {
            State = ImportBatchState.FAILED;
        }
    }
}
