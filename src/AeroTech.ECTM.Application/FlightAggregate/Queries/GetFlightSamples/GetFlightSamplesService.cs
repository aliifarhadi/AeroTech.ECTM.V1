using AeroTech.ECTM.Application.FlightAggregate.Dto;
using AeroTech.ECTM.Application.ImportBatchAggregate.Services;
using AeroTech.ECTM.Domain._Shared.Recording;
using AeroTech.ECTM.Domain._Shared.Resources;
using AeroTech.ECTM.Domain.FlightAggregate.Contracts;
using AeroTech.ECTM.Domain.SourceFileAggregate.Contracts;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Application.FlightAggregate.Queries.GetFlightSamples
{
    public sealed class GetFlightSamplesService : IGetFlightSamplesService
    {
        public const int MaximumRows = 500;

        private readonly IFlightRepository _flights;
        private readonly ISourceFileRepository _sourceFiles;
        private readonly RecordingSetLoader _recordingSets;
        private readonly ImportOptions _options;

        public GetFlightSamplesService(IFlightRepository flights, ISourceFileRepository sourceFiles, RecordingSetLoader recordingSets, IOptions<ImportOptions> options)
        {
            _flights = flights;
            _sourceFiles = sourceFiles;
            _recordingSets = recordingSets;
            _options = options.Value;
        }

        public async Task<BackofficeFlightSamplesDto> ExecuteAsync(IFlightSamplesQuery query, CancellationToken cancellationToken = default)
        {
            var flight = await _flights.FindAsync(query.FlightId, cancellationToken)
                         ?? throw ExceptionFactory.FlightNotFound(query.FlightId);
            var sourceFile = await _sourceFiles.GetAsync(flight.SourceFileId, cancellationToken)
                             ?? throw ExceptionFactory.FlightNotFound(query.FlightId);
            var bytes = await _sourceFiles.GetBytesAsync(flight.SourceFileId, cancellationToken)
                        ?? throw ExceptionFactory.FlightNotFound(query.FlightId);
            var recordingSet = await _recordingSets.LoadPinnedAsync(flight.Versions, cancellationToken);

            var lastIndex = flight.SampleCount - 1;
            var from = Math.Min(query.From ?? 0, lastIndex);
            var to = Math.Min(query.To ?? from + MaximumRows - 1, lastIndex);

            var rows = SourceRows.Read(sourceFile.FileName, bytes, recordingSet.Delimiter, _options.MaximumFileBytes);
            var header = new List<string>();
            var sampleRows = new List<BackofficeSampleRowDto>();
            var sampleIndex = -1;

            foreach (var row in rows)
            {
                if (sampleIndex == -1)
                    header.AddRange(row);

                if (sampleIndex >= from && sampleIndex <= to)
                    sampleRows.Add(new BackofficeSampleRowDto(sampleIndex, row));

                if (sampleIndex == to)
                    break;

                sampleIndex++;
            }

            return new BackofficeFlightSamplesDto(flight.Id, from, to, header, sampleRows);
        }
    }
}
