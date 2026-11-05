using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Services;
using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain._Shared;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Computation;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using AeroTech.Messages.Ectm.Enums;

namespace AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Queries.DryRunOnFlight
{
    public sealed class DryRunOnFlightService : IDryRunOnFlightService
    {
        public const string Added = "ADDED";
        public const string Unchanged = "UNCHANGED";
        public const string Changed = "CHANGED";

        private const string ProfileDryRunPhase = "P02 (dry-run with a file)";

        private readonly ConfigurationDocumentLoader _documentLoader;
        private readonly IConfigurationDocumentRepository _documents;
        private readonly IFlightRepository _flights;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly RecordingInspector _inspector;

        public DryRunOnFlightService(
            ConfigurationDocumentLoader documentLoader,
            IConfigurationDocumentRepository documents,
            IFlightRepository flights,
            ISourceFileRepository sourceFiles,
            RecordingInspector inspector)
        {
            _documentLoader = documentLoader;
            _documents = documents;
            _flights = flights;
            _sourceFiles = sourceFiles;
            _inspector = inspector;
        }

        public async Task<BackofficeFlightDryRunDto> ExecuteAsync(string kind, long documentId, long flightId, int budget, CancellationToken cancellationToken = default)
        {
            var document = await _documentLoader.LoadAsync(kind, documentId, cancellationToken);

            if (document.Kind != ConfigurationKind.PARAMETER_CATALOG)
                throw ExceptionFactory.DryRunNotAvailable(document.Kind, DryRunPhaseOf(document.Kind));

            var flight = await _flights.GetAsync(flightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(flightId);

            var catalogValidation = ParameterCatalogValidator.Validate(document.Content);

            if (!catalogValidation.IsValid)
                throw ExceptionFactory.ConfigurationDocumentIsInvalid(document.Id, catalogValidation.Issues.Count);

            var catalog = catalogValidation.Catalog!;
            var profileDocument = await _documents.GetAsync(flight.DataProfileDocumentId, cancellationToken)
                                  ?? throw ExceptionFactory.ConfigurationDocumentNotFound(flight.DataProfileDocumentId);
            var profileValidation = DataProfileValidator.Validate(profileDocument.Content, catalog, forPublish: false);
            var versions = new ConfigurationVersions(profileDocument.Id, profileDocument.VersionNumber, document.Id, document.VersionNumber);
            var active = await ActiveCatalogAsync(cancellationToken);
            var result = new DryRun(document, flight, versions, active?.Document);

            if (!profileValidation.IsValid)
                return result.WithIssues(profileValidation.Issues);

            var recordingSet = new RecordingSet(versions, profileValidation.Profile!, catalog);
            var inspection = await InspectAsync(flight, recordingSet, cancellationToken);

            if (inspection.IsRejected)
                return result.WithIssues([new ValidationIssue("$", inspection.RejectionCode!, inspection.RejectionMessage!)]);

            var recording = inspection.Recording!;
            var derived = DerivedSeriesEvaluator.Evaluate(catalog, recording.Channels, recording.SampleCount);
            var activeDerived = DerivedOfActive(active?.Catalog, recording);
            var findings = FindingComparison.Compare(flight.Findings(), recording.Findings);

            return result.WithResults(recording, findings, derived, activeDerived, budget);
        }

        private async Task<RecordingInspection> InspectAsync(Flight flight, RecordingSet recordingSet, CancellationToken cancellationToken)
        {
            var sourceFile = await _sourceFiles.GetAsync(flight.SourceFileId, cancellationToken)
                             ?? throw ExceptionFactory.FlightNotFound(flight.Id);
            var bytes = await _sourceFiles.GetBytesAsync(flight.SourceFileId, cancellationToken)
                        ?? throw ExceptionFactory.FlightNotFound(flight.Id);

            return _inspector.Inspect(sourceFile.FileName, bytes, recordingSet);
        }

        private async Task<ActiveCatalog?> ActiveCatalogAsync(CancellationToken cancellationToken)
        {
            var document = await _documents.FindActiveAsync(ConfigurationKind.PARAMETER_CATALOG, cancellationToken);

            if (document is null)
                return null;

            var catalog = ParameterCatalogValidator.Validate(document.Content).Catalog;

            return new ActiveCatalog(document, catalog);
        }

        private static IReadOnlyList<DerivedSeries> DerivedOfActive(ValidatedCatalog? activeCatalog, ParsedRecording recording)
        {
            if (activeCatalog is null)
                return [];

            return DerivedSeriesEvaluator.Evaluate(activeCatalog, recording.Channels, recording.SampleCount);
        }

        private static string DryRunPhaseOf(ConfigurationKind kind)
        {
            if (kind == ConfigurationKind.DATA_PROFILE)
                return ProfileDryRunPhase;

            return ConfigurationKinds.DeliveryPhase[kind];
        }

        private sealed record ActiveCatalog(ConfigurationDocument Document, ValidatedCatalog? Catalog);

        private sealed record DryRun(ConfigurationDocument Document, Flight Flight, ConfigurationVersions Versions, ConfigurationDocument? Active)
        {
            public BackofficeFlightDryRunDto WithIssues(IReadOnlyList<ValidationIssue> issues)
                => DtoOf(issues.Select(BackofficeValidationIssueDto.From).ToList(), 0, [], [], []);

            public BackofficeFlightDryRunDto WithResults(
                ParsedRecording recording,
                IReadOnlyList<FindingChange> findings,
                IReadOnlyList<DerivedSeries> derived,
                IReadOnlyList<DerivedSeries> activeDerived,
                int budget)
            {
                var activeByKey = activeDerived.ToDictionary(series => series.ParameterKey, StringComparer.Ordinal);
                var elapsed = recording.TimeChannel?.Numbers;
                var derivedDtos = derived
                    .Select(series => BackofficeDerivedDryRunDto.From(series, AgainstActive(series, activeByKey), Sampled(series, elapsed, budget)))
                    .ToList();
                var derivedKeys = derived.Select(series => series.ParameterKey).ToHashSet(StringComparer.Ordinal);
                var removed = activeDerived
                    .Select(series => series.ParameterKey)
                    .Where(key => !derivedKeys.Contains(key))
                    .ToList();

                return DtoOf([], recording.SampleCount, findings.Select(BackofficeFindingChangeDto.From).ToList(), derivedDtos, removed);
            }

            private BackofficeFlightDryRunDto DtoOf(
                IReadOnlyList<BackofficeValidationIssueDto> issues,
                int sampleCount,
                IReadOnlyList<BackofficeFindingChangeDto> findings,
                IReadOnlyList<BackofficeDerivedDryRunDto> derived,
                IReadOnlyList<string> removed)
                => new(
                    Document.Id,
                    EnumValueDto.Of(Document.Kind),
                    Document.VersionNumber,
                    EnumValueDto.Of(Document.Status),
                    Flight.Id,
                    BackofficeConfigurationVersionsDto.From(Flight.Versions),
                    BackofficeConfigurationVersionsDto.From(Versions),
                    Active?.Id,
                    Active?.VersionNumber,
                    issues,
                    sampleCount,
                    findings,
                    derived,
                    removed);

            private static string AgainstActive(DerivedSeries series, IReadOnlyDictionary<string, DerivedSeries> activeByKey)
            {
                if (!activeByKey.TryGetValue(series.ParameterKey, out var active))
                    return Added;

                return series.SameValuesAs(active) ? Unchanged : Changed;
            }

            private static SampledSeries? Sampled(DerivedSeries series, IReadOnlyList<double?>? elapsed, int budget)
            {
                if (elapsed is null || series.Values.Count == 0)
                    return null;

                return SeriesSampler.Sample(elapsed, series.Values, 0, series.Values.Count - 1, SeriesSampler.M4, budget);
            }
        }
    }
}
