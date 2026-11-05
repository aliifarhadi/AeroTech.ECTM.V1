using AeroTech.ECTM.Application._Shared.Enums;
using AeroTech.ECTM.Application.ConfigurationDocumentAggregate.Dto;
using AeroTech.ECTM.Domain.FlightPhaseAssessmentAggregate.Computation;

namespace AeroTech.ECTM.Application.FlightPhaseAssessmentAggregate.Dto
{
    public sealed record BackofficePhaseReferenceDto(
        string Algorithm,
        string Title,
        EnumValueDto Match,
        IReadOnlyList<BackofficeJsonChangeDto> Changes)
    {
        public static BackofficePhaseReferenceDto From(PhaseReferenceComparison reference)
            => new(reference.Algorithm, reference.Title, EnumValueDto.Of(reference.Match), reference.Changes.Select(BackofficeJsonChangeDto.From).ToList());
    }
}
