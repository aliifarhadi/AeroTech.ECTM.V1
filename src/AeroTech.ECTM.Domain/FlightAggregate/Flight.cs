using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Arguments;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Entities;
using AeroTech.Framework.Core.Domain.Aggregates;
using AeroTech.Framework.Core.ServiceContracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Domain.FlightAggregate
{
    public sealed class Flight : AggregateRoot<long>
    {
        public const string IdentityChange = "IDENTITY";

        private readonly List<FlightEngineAssignment> _engines = new();
        private readonly List<FlightValidationIssue> _issues = new();
        private readonly List<FlightChannel> _channels = new();

        private Flight()
        {
        }

        private Flight(long id, FlightRecordingArgs recording, DateTimeOffset importedAt)
        {
            Id = id;
            SourceFileId = recording.SourceFileId;
            ImportBatchId = recording.ImportBatchId;
            ImportItemId = recording.ImportItemId;
            DataProfileDocumentId = recording.Versions.DataProfileDocumentId;
            DataProfileVersion = recording.Versions.DataProfileVersion;
            ParameterCatalogDocumentId = recording.Versions.ParameterCatalogDocumentId;
            ParameterCatalogVersion = recording.Versions.ParameterCatalogVersion;
            TimeParameterKey = recording.TimeParameterKey;
            SampleCount = recording.SampleCount;
            FirstElapsed = recording.FirstElapsed;
            LastElapsed = recording.LastElapsed;
            ImportedAt = importedAt;
        }

        public long SourceFileId { get; private set; }

        public long ImportBatchId { get; private set; }

        public long ImportItemId { get; private set; }

        public long? AircraftId { get; private set; }

        public string? Registration { get; private set; }

        public IdentitySource? RegistrationSource { get; private set; }

        public DateOnly? FlightDate { get; private set; }

        public IdentitySource? FlightDateSource { get; private set; }

        public string? FlightNumber { get; private set; }

        public IdentitySource? FlightNumberSource { get; private set; }

        public IdentityStatus IdentityStatus { get; private set; }

        public List<string> IdentityReasons { get; private set; } = new();

        public bool DistinctFlightConfirmed { get; private set; }

        public string? IdentityNote { get; private set; }

        public FlightState State { get; private set; }

        public FlightState? StateBeforeHold { get; private set; }

        public string? HoldNote { get; private set; }

        public DateTimeOffset? HeldAt { get; private set; }

        public long DataProfileDocumentId { get; private set; }

        public int DataProfileVersion { get; private set; }

        public long ParameterCatalogDocumentId { get; private set; }

        public int ParameterCatalogVersion { get; private set; }

        public string? TimeParameterKey { get; private set; }

        public int SampleCount { get; private set; }

        public double? FirstElapsed { get; private set; }

        public double? LastElapsed { get; private set; }

        public int BlockingCount { get; private set; }

        public int WarningCount { get; private set; }

        public int InfoCount { get; private set; }

        public DateTimeOffset ImportedAt { get; private set; }

        public IReadOnlyCollection<FlightEngineAssignment> Engines => _engines.AsReadOnly();

        public IReadOnlyCollection<FlightValidationIssue> Issues => _issues.AsReadOnly();

        public IReadOnlyCollection<FlightChannel> Channels => _channels.AsReadOnly();

        public IReadOnlyList<int> Positions => _engines.Select(engine => (int)engine.Position).Order().ToList();

        public ConfigurationVersions Versions => new(DataProfileDocumentId, DataProfileVersion, ParameterCatalogDocumentId, ParameterCatalogVersion);

        public IReadOnlyList<ValidationFinding> Findings()
            => _issues
                .Select(issue => new ValidationFinding(issue.Severity, issue.Code, issue.Message, issue.ParameterKey, issue.Count, issue.Value))
                .ToList();

        public static Flight Import(
            long id,
            FlightRecordingArgs recording,
            IdentityResolution identity,
            IReadOnlyList<ValidationFinding> findings,
            IReadOnlyList<RecordedChannel> channels,
            IIdGenerator idGenerator,
            DateTimeOffset now)
        {
            var flight = new Flight(id, recording, now);

            flight.ApplyIdentity(identity);

            foreach (var engine in identity.Engines)
                flight._engines.Add(new FlightEngineAssignment(idGenerator.NewId(), id, engine));

            foreach (var finding in findings)
                flight._issues.Add(new FlightValidationIssue(idGenerator.NewId(), id, finding));

            foreach (var channel in channels)
                flight._channels.Add(FlightChannel.Record(idGenerator.NewId(), id, channel));

            flight.BlockingCount = findings.Count(finding => finding.Severity == ValidationSeverity.BLOCKING);
            flight.WarningCount = findings.Count(finding => finding.Severity == ValidationSeverity.WARNING);
            flight.InfoCount = findings.Count(finding => finding.Severity == ValidationSeverity.INFO);
            flight.State = StateFor(identity.Status);

            return flight;
        }

        public void ConfirmIdentity(IdentityResolution identity, bool distinctFlightConfirmed, string? note)
        {
            if (State is not (FlightState.NEEDS_IDENTITY or FlightState.PENDING_REVIEW))
                throw ExceptionFactory.ReviewNotAllowed(IdentityChange, State);

            ApplyIdentity(identity);

            foreach (var engine in _engines)
                engine.Assign(identity.Engines.Single(resolution => resolution.Position == engine.Position));

            DistinctFlightConfirmed = distinctFlightConfirmed;
            IdentityNote = OptionalText.Normalize(note);
            State = StateFor(identity.Status);
        }

        public void Review(ReviewAction action, string? note, DateTimeOffset now)
        {
            switch (action)
            {
                case ReviewAction.HOLD:
                    Hold(note, now);
                    break;
                case ReviewAction.RELEASE:
                    Release();
                    break;
                case ReviewAction.REQUEST_IDENTITY:
                    RequestIdentity(note);
                    break;
                case ReviewAction.ACCEPT:
                    Decide(ReviewAction.ACCEPT, FlightState.ACCEPTED);
                    break;
                case ReviewAction.EXCLUDE:
                    Decide(ReviewAction.EXCLUDE, FlightState.EXCLUDED);
                    break;
                default:
                    throw ExceptionFactory.ReviewNotAllowed(action, State);
            }
        }

        public void ReturnToReview()
        {
            if (State is FlightState.ACCEPTED or FlightState.EXCLUDED)
                State = FlightState.PENDING_REVIEW;
        }

        public string EngineAssignmentKey()
            => string.Join("|", _engines
                .OrderBy(engine => engine.Position)
                .Select(engine => $"{engine.Position}:{engine.Status}:{engine.EngineId?.ToString() ?? "-"}"));

        private void Decide(ReviewAction action, FlightState decided)
        {
            if (State != FlightState.PENDING_REVIEW)
                throw ExceptionFactory.ReviewNotAllowed(action, State);

            State = decided;
        }

        private void Hold(string? note, DateTimeOffset now)
        {
            if (State is not (FlightState.PENDING_REVIEW or FlightState.NEEDS_IDENTITY or FlightState.ACCEPTED))
                throw ExceptionFactory.ReviewNotAllowed(ReviewAction.HOLD, State);

            StateBeforeHold = State == FlightState.ACCEPTED ? FlightState.PENDING_REVIEW : State;
            State = FlightState.HELD;
            HoldNote = OptionalText.Normalize(note);
            HeldAt = now;
        }

        private void Release()
        {
            if (State != FlightState.HELD)
                throw ExceptionFactory.ReviewNotAllowed(ReviewAction.RELEASE, State);

            State = StateBeforeHold!.Value;
            StateBeforeHold = null;
            HoldNote = null;
            HeldAt = null;
        }

        private void RequestIdentity(string? note)
        {
            if (State is not (FlightState.PENDING_REVIEW or FlightState.ACCEPTED))
                throw ExceptionFactory.ReviewNotAllowed(ReviewAction.REQUEST_IDENTITY, State);

            IdentityStatus = IdentityStatus.NEEDS_CONFIRMATION;
            IdentityReasons = IdentityReasons.Append(Computation.IdentityReasons.IdentityReviewRequested).ToList();
            IdentityNote = OptionalText.Normalize(note);
            State = FlightState.NEEDS_IDENTITY;
        }

        private void ApplyIdentity(IdentityResolution identity)
        {
            AircraftId = identity.AircraftId;
            Registration = identity.Registration;
            RegistrationSource = identity.RegistrationSource;
            FlightDate = identity.FlightDate;
            FlightDateSource = identity.FlightDateSource;
            FlightNumber = identity.FlightNumber;
            FlightNumberSource = identity.FlightNumberSource;
            IdentityStatus = identity.Status;
            IdentityReasons = identity.Reasons.ToList();
        }

        private static FlightState StateFor(IdentityStatus identityStatus)
        {
            if (identityStatus == IdentityStatus.RESOLVED)
                return FlightState.PENDING_REVIEW;

            return FlightState.NEEDS_IDENTITY;
        }
    }
}
