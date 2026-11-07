using AeroTech.ECTM.Domain.BaselineParameterFlightAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Computation;
using AeroTech.ECTM.Domain.ConfigurationDocumentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate;
using AeroTech.ECTM.Domain.FlightStabilityPointsAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;

namespace AeroTech.ECTM.Application.BaselineParameterFlightAggregate.Services
{
    public sealed class FlightEvidenceLoader
    {
        private readonly IFlightRepository _flights;
        private readonly IFlightStabilityPointsRepository _stabilityPoints;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly IConfigurationDocumentRepository _documents;
        private readonly Dictionary<long, ValidatedCatalog?> _catalogByDocument = new();

        public FlightEvidenceLoader(
            IFlightRepository flights,
            IFlightStabilityPointsRepository stabilityPoints,
            ISourceFileRepository sourceFiles,
            IConfigurationDocumentRepository documents)
        {
            _flights = flights;
            _stabilityPoints = stabilityPoints;
            _sourceFiles = sourceFiles;
            _documents = documents;
        }

        public async Task<IReadOnlyList<FlightEvidence>> ForEngineAsync(long engineId, CancellationToken cancellationToken = default)
        {
            var flights = await _flights.ListWithEngineBetweenAsync(engineId, null, null, cancellationToken);

            return await EvidenceOfAsync(flights, engineId, cancellationToken);
        }

        private async Task<IReadOnlyList<FlightEvidence>> EvidenceOfAsync(IReadOnlyList<Flight> flights, long engineId, CancellationToken cancellationToken)
        {
            var flightById = flights.ToDictionary(flight => flight.Id);
            var points = await _stabilityPoints.ListCurrentAsync(flightById.Keys.ToList(), cancellationToken);
            var sourceFiles = await _sourceFiles.ListByIdsAsync(flights.Select(flight => flight.SourceFileId).ToList(), cancellationToken);
            var sha256BySourceFile = sourceFiles.ToDictionary(sourceFile => sourceFile.Id, sourceFile => sourceFile.Sha256);
            var evidence = new List<FlightEvidence>();

            foreach (var flightPoints in points)
            {
                var flight = flightById[flightPoints.FlightId];
                var source = SourceOf(flight, flightPoints, engineId, sha256BySourceFile.GetValueOrDefault(flight.SourceFileId));
                var catalog = await CatalogOfAsync(flight.ParameterCatalogDocumentId, cancellationToken);

                evidence.Add(new FlightEvidence(flight, flightPoints, flightPoints.ReadResult(), source, catalog));
            }

            return evidence
                .OrderBy(item => item.Flight.FlightDate)
                .ThenBy(item => item.Flight.Id)
                .ToList();
        }

        private static FlightSource SourceOf(Flight flight, FlightStabilityPoints points, long engineId, string? sourceFileSha256)
        {
            var position = flight.Engines.First(engine => engine.EngineId == engineId).Position;

            return new FlightSource(
                engineId,
                flight.Id,
                flight.FlightDate,
                sourceFileSha256,
                points.AnalysisRevisionId,
                points.Id,
                points.SnapshotRulesDocumentId,
                points.SnapshotRulesVersion,
                position);
        }

        private async Task<ValidatedCatalog?> CatalogOfAsync(long catalogDocumentId, CancellationToken cancellationToken)
        {
            if (_catalogByDocument.TryGetValue(catalogDocumentId, out var cached))
                return cached;

            var document = await _documents.GetAsync(catalogDocumentId, cancellationToken);
            var catalog = document is null ? null : ParameterCatalogValidator.Validate(document.Content).Catalog;

            _catalogByDocument[catalogDocumentId] = catalog;

            return catalog;
        }
    }
}
