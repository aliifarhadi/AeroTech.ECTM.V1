using AeroTech.ECTM.Domain.EngineAggregate;

namespace AeroTech.ECTM.Application.EngineAggregate.Dto
{
    public sealed record BackofficeConfigurationEvidenceDto(
        string Reference,
        DateTimeOffset? DocumentedUtc,
        string? DocumentedBy)
    {
        public static BackofficeConfigurationEvidenceDto? From(Engine engine)
        {
            if (engine.ConfigurationEvidenceReference is not { } reference)
                return null;

            return new(reference, engine.ConfigurationDocumentedAt, engine.ConfigurationDocumentedBy);
        }
    }
}
