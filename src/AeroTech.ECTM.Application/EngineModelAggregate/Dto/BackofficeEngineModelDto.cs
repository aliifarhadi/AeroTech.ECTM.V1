using AeroTech.ECTM.Domain.EngineModelAggregate;

namespace AeroTech.ECTM.Application.EngineModelAggregate.Dto
{
    public sealed record BackofficeEngineModelDto(
        long Id,
        string Code,
        string Manufacturer,
        string? Family,
        string? VariantCode,
        string? ThrustRatingCode,
        IReadOnlyList<string> DocumentReferences,
        string? Notes)
    {
        public static BackofficeEngineModelDto From(EngineModel engineModel)
            => new(
                engineModel.Id,
                engineModel.Code,
                engineModel.Manufacturer,
                engineModel.Family,
                engineModel.VariantCode,
                engineModel.ThrustRatingCode,
                engineModel.DocumentReferences.ToList(),
                engineModel.Notes);
    }
}
