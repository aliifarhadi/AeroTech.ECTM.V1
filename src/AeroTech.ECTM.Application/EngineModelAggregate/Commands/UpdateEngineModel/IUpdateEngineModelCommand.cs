namespace AeroTech.ECTM.Application.EngineModelAggregate.Commands.UpdateEngineModel
{
    public interface IUpdateEngineModelCommand
    {
        long EngineModelId { get; }

        string Manufacturer { get; }

        string? Family { get; }

        string? VariantCode { get; }

        string? ThrustRatingCode { get; }

        IReadOnlyList<string> DocumentReferences { get; }

        string? Notes { get; }
    }
}
