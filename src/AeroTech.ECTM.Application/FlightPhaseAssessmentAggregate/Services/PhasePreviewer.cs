using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.AnalysisRevisionAggregate.Services;
using AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto;
using AeroTech.ECTM.Application.FlightQualityAggregate.Services;
using AeroTech.ECTM.Domain.AnalysisRevisionAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightAggregate;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Contracts;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Computation;
using AeroTech.ECTM.Domain.FlightQualityAggregate.Contracts;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Services
{
    public sealed class PhasePreviewer
    {
        private readonly IAnalysisRevisionRepository _revisions;
        private readonly IFlightQualityRepository _qualities;
        private readonly IFlightPhaseAssessmentRepository _phases;
        private readonly FlightQualityInputLoader _inputs;

        public PhasePreviewer(
            IAnalysisRevisionRepository revisions,
            IFlightQualityRepository qualities,
            IFlightPhaseAssessmentRepository phases,
            FlightQualityInputLoader inputs)
        {
            _revisions = revisions;
            _qualities = qualities;
            _phases = phases;
            _inputs = inputs;
        }

        public async Task<BackofficePhaseDryRunDto> PreviewAsync(Flight flight, AnalysisRules rules, CancellationToken cancellationToken = default)
        {
            var revision = await _revisions.FindCurrentAsync(flight.Id, cancellationToken);
            var currentQuality = await _qualities.FindCurrentAsync(flight.Id, cancellationToken);
            var currentAssessment = await _phases.FindCurrentAsync(flight.Id, cancellationToken);
            var input = await _inputs.LoadAsync(flight, rules.ParameterCodes, cancellationToken);
            var quality = QualityEvaluator.Evaluate(rules.Quality, input);
            var current = currentAssessment?.ReadResult();
            var proposed = rules.Phase is { } phaseRules ? PhaseEvaluator.Evaluate(phaseRules, input, quality) : null;
            var diff = PhaseComparison.Compare(current, proposed);

            return new BackofficePhaseDryRunDto(
                flight.Id,
                revision?.Id,
                revision?.Number,
                currentAssessment?.PhaseRuleSetDocumentId,
                currentAssessment?.PhaseRuleSetVersion,
                EnumValueDto.OfNullable(currentQuality?.Grade),
                rules.QualityDocument.Id,
                rules.QualityDocument.VersionNumber,
                rules.PhaseDocument?.Id,
                rules.PhaseDocument?.VersionNumber,
                EnumValueDto.OfNullable(rules.PhaseDocument?.Status),
                EnumValueDto.Of(quality.Grade),
                diff.ChangedSamples,
                diff.ChangedPercent,
                diff.Changes.Select(BackofficePhaseChangeDto.From).ToList(),
                diff.Anchors.Select(BackofficeAnchorChangeDto.From).ToList(),
                (current?.Coverage ?? []).Select(BackofficePhaseCoverageDto.From).ToList(),
                (proposed?.Coverage ?? []).Select(BackofficePhaseCoverageDto.From).ToList(),
                (proposed?.Segments ?? []).Select(BackofficePhaseSegmentDto.From).ToList(),
                (proposed?.Anchors ?? []).Select(BackofficeAnchorDto.From).ToList(),
                proposed is null ? null : BackofficePhaseReferenceDto.From(proposed.Reference));
        }
    }
}
