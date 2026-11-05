using AeroTech.ECTM.Domain.ImportBatchAggregate.Arguments;
using AeroTech.ECTM.Domain.ImportBatchAggregate.Computation;
using AeroTech.Framework.Core.Domain.Entities;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.ImportBatchAggregate.Entities
{
    public sealed class ImportItem : Entity<long>
    {
        private ImportItem()
        {
        }

        internal ImportItem(long id, long importBatchId, int ordinal, ImportItemArgs item)
        {
            Id = id;
            ImportBatchId = importBatchId;
            Ordinal = ordinal;
            FileName = item.FileName;
            ContainerName = item.ContainerName;
            SizeBytes = item.SizeBytes;
            Sha256 = item.Sha256;
            SourceFileId = item.SourceFileId;
            Outcome = item.Outcome;
            RejectionCode = item.RejectionCode;
            ExistingFlightId = item.ExistingFlightId;
            State = ImportItemState.PREVIEWED;
            Preview = item.Preview.ToJson();
        }

        public long ImportBatchId { get; private set; }

        public int Ordinal { get; private set; }

        public string FileName { get; private set; } = default!;

        public string? ContainerName { get; private set; }

        public long SizeBytes { get; private set; }

        public string? Sha256 { get; private set; }

        public long? SourceFileId { get; private set; }

        public ImportItemOutcome Outcome { get; private set; }

        public string? RejectionCode { get; private set; }

        public long? ExistingFlightId { get; private set; }

        public ImportItemState State { get; private set; }

        public long? FlightId { get; private set; }

        public string? ErrorCode { get; private set; }

        public string Preview { get; private set; } = default!;

        public bool IsPending => State is ImportItemState.PREVIEWED or ImportItemState.FAILED;

        public ImportItemPreview ReadPreview() => ImportItemPreview.FromJson(Preview);

        internal void Commit(long flightId)
        {
            Outcome = ImportItemOutcome.NEW;
            State = ImportItemState.COMMITTED;
            FlightId = flightId;
            ErrorCode = null;
        }

        internal void LinkDuplicate(long flightId)
        {
            Outcome = ImportItemOutcome.DUPLICATE;
            State = ImportItemState.SKIPPED;
            ExistingFlightId = flightId;
            FlightId = flightId;
            ErrorCode = null;
        }

        internal void Skip()
        {
            State = ImportItemState.SKIPPED;
        }

        internal void Fail(string errorCode)
        {
            State = ImportItemState.FAILED;
            ErrorCode = errorCode;
        }
    }
}
